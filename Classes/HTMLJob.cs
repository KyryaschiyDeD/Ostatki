using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Остатки.Classes
{
	public class HTMLJob
	{
		public static string[] userAgent = {
		"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.80 Safari/537.36",
"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.80 Safari/537.36",
"Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.80 Safari/537.36",
"Mozilla/5.0 (Macintosh; Intel Mac OS X 12_2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.80 Safari/537.36",
"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:96.0) Gecko/20100101 Firefox/96.0",
"Mozilla/5.0 (Macintosh; Intel Mac OS X 12.2; rv:96.0) Gecko/20100101 Firefox/96.0",
"Mozilla/5.0 (X11; Linux i686; rv:96.0) Gecko/20100101 Firefox/96.0",
"Mozilla/5.0 (Linux x86_64; rv:96.0) Gecko/20100101 Firefox/96.0",
"Mozilla/5.0 (X11; Ubuntu; Linux i686; rv:96.0) Gecko/20100101 Firefox/96.0",
"Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:96.0) Gecko/20100101 Firefox/96.0",
"Mozilla/5.0 (X11; Fedora; Linux x86_64; rv:96.0) Gecko/20100101 Firefox/96.0",
"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:91.0) Gecko/20100101 Firefox/91.0",
"Mozilla/5.0 (Macintosh; Intel Mac OS X 12.2; rv:91.0) Gecko/20100101 Firefox/91.0",
"Mozilla/5.0 (X11; Linux i686; rv:91.0) Gecko/20100101 Firefox/91.0",
"Mozilla/5.0 (Linux x86_64; rv:91.0) Gecko/20100101 Firefox/91.0",
"Mozilla/5.0 (X11; Ubuntu; Linux i686; rv:91.0) Gecko/20100101 Firefox/91.0",
"Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:91.0) Gecko/20100101 Firefox/91.0",
"Mozilla/5.0 (X11; Fedora; Linux x86_64; rv:91.0) Gecko/20100101 Firefox/91.0",
"Mozilla/5.0 (Macintosh; Intel Mac OS X 12_2) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/15.2 Safari/605.1.15",
"Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0)",
"Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0)",
"Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0)",
"Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.0; Trident/5.0)",
"Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)",
"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)",
"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)",
"Mozilla/5.0 (Windows NT 6.1; Trident/7.0; rv:11.0) like Gecko",
"Mozilla/5.0 (Windows NT 6.2; Trident/7.0; rv:11.0) like Gecko",
"Mozilla/5.0 (Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko",
"Mozilla/5.0 (Windows NT 10.0; Trident/7.0; rv:11.0) like Gecko",
"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.80 Safari/537.36 Edg/97.0.1072.69",
"Mozilla/5.0 (Macintosh; Intel Mac OS X 12_2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.80 Safari/537.36 Edg/97.0.1072.69",
"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.80 Safari/537.36 OPR/83.0.4254.16",
"Mozilla/5.0 (Windows NT 10.0; WOW64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.80 Safari/537.36 OPR/83.0.4254.16",
"Mozilla/5.0 (Macintosh; Intel Mac OS X 12_2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.80 Safari/537.36 OPR/83.0.4254.16",
"Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.80 Safari/537.36 OPR/83.0.4254.16",
"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.80 Safari/537.36 Vivaldi/4.3",
"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.80 Safari/537.36 Vivaldi/4.3",
"Mozilla/5.0 (Macintosh; Intel Mac OS X 12_2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.80 Safari/537.36 Vivaldi/4.3",
"Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.80 Safari/537.36 Vivaldi/4.3",
"Mozilla/5.0 (X11; Linux i686) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.80 Safari/537.36 Vivaldi/4.3",
"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.80 YaBrowser/22.1.0 Yowser/2.5 Safari/537.36",
"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.80 YaBrowser/22.1.0 Yowser/2.5 Safari/537.36",
"Mozilla/5.0 (Macintosh; Intel Mac OS X 12_2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.80 YaBrowser/22.1.0 Yowser/2.5 Safari/537.36",

		};
		public static string[] proxyIp = {
			"200.89.83.130",
"200.25.48.72",
"186.125.235.253",
"203.150.128.82",
"186.6.38.178",
"122.155.165.191",
"185.46.9.133",
"103.53.78.178",
"115.178.78.58",
"200.69.80.99",
"51.195.211.154",
"187.6.13.230",
"161.35.126.185",
"92.255.205.129",
"161.35.111.27",
"190.96.47.61",
"117.251.114.197",
"156.200.116.71",
"173.212.224.134",
"78.138.24.227",
"78.138.24.193",
"78.138.24.189",
"41.65.224.80",
"201.150.117.128",
"41.65.0.195",
"180.248.182.27",
"190.5.202.20",
"41.65.201.54",
"41.65.236.56",
"189.198.224.39",
"189.165.30.98",
"198.144.149.82",
"212.95.75.12",
"12.151.56.30",
"198.100.144.173",
"51.81.80.44",
"103.117.192.14",
"43.230.123.14",
"103.78.254.78",
"20.118.167.2",
"122.154.100.210",
"8.210.221.30",
"54.177.45.255",
"121.1.41.162",
"43.255.113.232",
"43.255.113.232",
"62.210.209.156",
"198.11.173.185",
"43.255.113.232",
"8.214.7.15",
"31.47.198.58",
"72.249.76.221",
"178.32.101.200",
"149.28.67.98",
"47.245.11.194",
"47.251.5.248",
"159.65.171.69",
"198.11.183.14",
"97.87.248.14",
"157.245.167.115",
"103.224.103.65",
"138.201.154.35",
"202.6.233.133",
"200.185.55.121",
"37.61.220.238",
"34.195.172.233",
"35.238.154.2",
"138.197.148.215",
"185.216.176.43",
"51.91.157.66",
"95.217.167.254",
"155.138.233.191",
"139.99.237.62",
"107.151.182.247",
"8.210.83.33",
"103.144.48.114",
"194.5.193.183",
"139.59.25.198",
"91.221.17.220",
"47.57.188.208",
"31.192.232.10",
"45.7.133.158",
"85.72.194.216",
"45.233.170.98",
"144.91.83.174",
"139.59.249.244",
"5.228.165.208",
"5.228.0.95",
"27.123.185.38",
"217.69.176.207",
"217.69.176.207",
"51.68.37.227",
"54.37.95.94",
"20.103.139.62",
"94.154.16.15",
"178.48.68.61",
"78.47.239.101",
"176.31.68.252",
"45.132.75.19",
"94.253.95.241",
"116.203.22.150",
"5.188.181.170",
"37.186.5.16",
"212.225.241.61",
"116.203.22.150",
"195.2.71.201",
"116.203.22.150",
"185.230.205.196",
"134.122.47.78",
"194.233.73.107",
"47.254.23.12",
"194.233.73.104",
"194.233.73.106",
"47.242.168.35",
"190.226.241.67",
"43.128.44.107",
"200.114.187.233",
"181.41.247.113",
"14.192.129.140",
"46.246.82.3",
"149.28.67.98",
"149.28.67.98",
"185.95.186.247",
"94.228.204.229",
"173.82.119.184",
"155.94.235.233",
"157.230.34.152",
"189.208.163.224",
"157.230.34.152",
"192.111.135.21",
"98.162.25.23",
"128.199.150.88",
"94.230.166.197",
"167.179.45.50",
"5.135.176.161",
"192.111.137.35",
"150.242.182.98",
"66.150.130.171",
"192.252.214.20",
"91.207.60.46",
"154.16.167.72",
"146.56.112.236",
"212.3.70.137",
"207.180.207.195",
"206.189.15.100",
"206.189.118.100",
"195.94.28.14",
"103.19.1.229",
"150.230.142.10",
"204.44.94.70",
"104.223.15.47",

		};
		public static int[] proxyPort = {
			3128,
3128,
999,
8080,
999,
3128,
3128,
8080,
8080,
8080,
8080,
3128,
5566,
8080,
5566,
3128,
3128,
1981,
3128,
3128,
3128,
3128,
1976,
999,
1981,
3128,
999,
8080,
1976,
80,
10101,
3128,
80,
80,
80,
80,
80,
80,
80,
8080,
80,
80,
80,
111,
84,
82,
80,
80,
86,
80,
80,
3128,
80,
24022,
80,
80,
80,
80,
80,
80,
15402,
24000,
80,
9090,
1080,
80,
80,
80,
8888,
80,
80,
80,
80,
80,
80,
80,
80,
80,
8000,
80,
5006,
999,
8080,
999,
3128,
7777,
8081,
8081,
8080,
8080,
80,
42003,
40032,
3128,
5678,
4145,
10042,
20150,
18868,
3629,
10026,
3080,
5678,
3629,
10031,
16072,
10024,
3128,
1080,
443,
7328,
443,
443,
9090,
3629,
808,
1080,
4153,
8080,
8888,
24061,
24028,
1080,
41890,
13714,
11186,
34718,
4153,
35481,
4145,
4145,
40035,
1080,
55443,
10000,
4145,
80,
58401,
15864,
46296,
50204,
45544,
2583,
10808,
21911,
64309,
8080,
25560,
1080,
56955,
48217,
		};

		private static int countOfUserAgent = 0;

		public static int CountOfUserAgent {
			get 
			{
				return countOfUserAgent;
			}

			set 
			{
				if (value > userAgent.Length - 1)
					countOfUserAgent = 0;
				else
					countOfUserAgent = value;
			} 
		}
		private static int countproxyIp = 0;
		public static int CountproxyIp { 
			get
			{
				return countproxyIp;
			}
			set 
			{
				if (value > proxyIp.Length - 1)
					countproxyIp = 0;
				else
					countproxyIp = value;
			} 
		}

		private static int countproxyPort = 0;
		public static int CountproxyPort { 
			get
			{
				return countproxyPort;
			}
			set
			{
				if (value > proxyPort.Length - 1)
					countproxyPort = 0;
				else
					countproxyPort = value;
			}
		}

		private static readonly Regex _tags_ = new Regex(@"<[^>]+?>", RegexOptions.Multiline | RegexOptions.Compiled);

		//add characters that are should not be removed to this regex
		private static readonly Regex _notOkCharacter_ = new Regex(@"[^\w&#@:/\?=|%!() -]", RegexOptions.Compiled);

		public static String UnHtml(String html)
		{
			html = HttpUtility.UrlDecode(html);
			html = HttpUtility.HtmlDecode(html);

			html = RemoveTag(html, "<!--", "-->");
			html = RemoveTag(html, "<script", "</script>");
			html = RemoveTag(html, "<style", "</style>");

			//replace matches of these regexes with space
			html = _tags_.Replace(html, " ");
			html = _notOkCharacter_.Replace(html, " ");
			html = SingleSpacedTrim(html);

			return html;
		}

		private static String RemoveTag(String html, String startTag, String endTag)
		{
			Boolean bAgain;
			do
			{
				bAgain = false;
				Int32 startTagPos = html.IndexOf(startTag, 0, StringComparison.CurrentCultureIgnoreCase);
				if (startTagPos < 0)
					continue;
				Int32 endTagPos = html.IndexOf(endTag, startTagPos + 1, StringComparison.CurrentCultureIgnoreCase);
				if (endTagPos <= startTagPos)
					continue;
				html = html.Remove(startTagPos, endTagPos - startTagPos + endTag.Length);
				bAgain = true;
			} while (bAgain);
			return html;
		}

		private static String SingleSpacedTrim(String inString)
		{
			StringBuilder sb = new StringBuilder();
			Boolean inBlanks = false;
			foreach (Char c in inString)
			{
				switch (c)
				{
					case '\r':
					case '\n':
					case '\t':
					case ' ':
						if (!inBlanks)
						{
							inBlanks = true;
							sb.Append(' ');
						}
						continue;
					default:
						inBlanks = false;
						sb.Append(c);
						break;
				}
			}
			return sb.ToString().Trim();
		}

	}
}
