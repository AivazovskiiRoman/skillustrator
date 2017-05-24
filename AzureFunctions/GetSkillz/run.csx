using System.Net;
using System.Collections.Generic;


public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request to get skills.");

    List<string> listOfSkills = new List<string>()
    {
        "CSS",
        "JavaScript",
        "C#",
        ".NET Core",
        "Angular",
        "React",
        "jQuery",
        "Python",
        "Django",
        "Flask",
        "Ruby",
        "Rails"
    };

    string skillsText = string.Join<string>(",", listOfSkills );

    // parse query parameter
    /*
    string name = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
        .Value;
     */

    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();

    // Set name to query string or body data
    //name = name ?? data?.name;


    return req.CreateResponse(HttpStatusCode.OK, listOfSkills);

    /*
    return name == null
        ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
        : req.CreateResponse(HttpStatusCode.OK, "Hello " + name);
    */
}