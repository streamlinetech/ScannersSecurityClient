using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Streamline.Security.Scanners.Core
{
	public static class JsonResourceRequestExtensions
	{
		const string DefaultAcceptHeader = "application/json, text/json;q=0.9";

		public static HttpWebRequest MakeResourceRequest(this Uri uri)
		{
			return MakeResourceRequest(uri, false);
		}

		public static HttpWebRequest MakeResourceRequest(this Uri uri, bool keepAlive)
		{
			var req = (HttpWebRequest)WebRequest.Create(uri);
			if (!keepAlive)
				req.KeepAlive = false;

			if (String.IsNullOrEmpty(req.Accept))
				req.Accept = DefaultAcceptHeader;

			return req;
		}

		public static T Get<T>(this HttpWebRequest request, Action<Exception, HttpWebResponse> after)
		{
			var data = default(T);
			ExecuteHttpVerb(request, "GET", (exception, response) =>
										{
											if (exception != null)
												throw new Exception(response.StatusDescription, exception);

											if (response.StatusCode.IsSuccess())
												data = response.DeserializeResponse<T>();
										});
			return data;
		}

		public static void Post<TBody>(this HttpWebRequest request, TBody body, Action<Exception, HttpWebResponse> after)
		{
			Post(request, body, Encoding.UTF8, after);
		}

		public static void Post<TBody>(this HttpWebRequest request, TBody body, Encoding encoding, Action<Exception, HttpWebResponse> after)
		{
			var lclEncoding = encoding ?? Encoding.UTF8;
			var bodyAsString = body.ToJson();
			var buffer = lclEncoding.GetBytes(bodyAsString);
			ExecuteHttpVerbWithPostBody(request, buffer, "application/json", "POST", after);
		}

		public static void Put<TBody>(this HttpWebRequest request, TBody body, Action<Exception, HttpWebResponse> after)
		{
			Put(request, body, Encoding.UTF8, after);
		}

		public static void Put<TBody>(this HttpWebRequest request, TBody body, Encoding encoding, Action<Exception, HttpWebResponse> after)
		{

			var lclEncoding = encoding ?? Encoding.UTF8;
			var bodyAsString = body.ToJson();
			var buffer = lclEncoding.GetBytes(bodyAsString);
			ExecuteHttpVerbWithPostBody(request, buffer, "application/json", "PUT", after);
		}

		public static string GetResponseBodyAsString(this HttpWebResponse response)
		{
			if (response != null)
			{
				using (var stream = response.GetResponseStream())
				{
					if (stream != null)
					{
						using (var reader = new StreamReader(stream))
						{
							return reader.ReadToEnd();
						}
					}
				}
			}
			return null;
		}

		public static bool IsSuccess(this HttpStatusCode code)
		{
			var c = (int)code;
			return c >= 200 && c < 300;
		}

		public static T DeserializeResponse<T>(this HttpWebResponse response)
		{
			return response.GetResponseBodyAsString().FromJson<T>();
		}

		public static string ToJson<T>(this T obj)
		{
			var serializer = new DataContractJsonSerializer(typeof(T));
			using (var memoryStream = new MemoryStream())
			{
				serializer.WriteObject(memoryStream, obj);
				return Encoding.Default.GetString(memoryStream.ToArray());
			}
		}

		public static T FromJson<T>(this string json)
		{
			using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(json)))
			{
				var serializer = new DataContractJsonSerializer(typeof(T));
				return (T)serializer.ReadObject(stream);
			}
		}

		static void ExecuteHttpVerb(HttpWebRequest request, string httpVerb, Action<Exception, HttpWebResponse> after)
		{

			WebResponse response = null;
			try
			{
				request.Method = httpVerb;
				try
				{
					response = request.GetResponse();
				}
				catch (WebException webException)
				{
					if (response == null)
						after(webException, webException.Response as HttpWebResponse);
				}
				after(null, (HttpWebResponse)response);
			}
			finally
			{
				if (response != null)
					response.Close();
			}
		}

		static void ExecuteHttpVerbWithPostBody(HttpWebRequest request, byte[] jsonBody, string contentType, string verb, Action<Exception, HttpWebResponse> after)
		{
			WebResponse response = null;
			try
			{
				request.Method = verb;
				request.ContentType = contentType;
				request.ContentLength = jsonBody.Length;

				using (var postData = request.GetRequestStream())
					postData.Write(jsonBody, 0, jsonBody.Length);
			}
			catch (Exception e)
			{
				after(e, null);
				return;
			}
			try
			{
				try
				{
					response = request.GetResponse();
				}
				catch (WebException webException)
				{
					if (response == null)
						after(webException, webException.Response as HttpWebResponse);
				}
				after(null, (HttpWebResponse)response);
			}
			finally
			{
				if (response != null)
					response.Close();
			}
		}
	}
}