// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See full license at the bottom of this file. 
using Microsoft.ProjectOxford.Face;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace twinsornot.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Upload()
        {
            try {
                if (Request.Files.Count != 2)
                {
                    return Json(new { error = "Error: I need two photos. Not more, not less. Two." });
                }

                FaceServiceClient client = new FaceServiceClient(WebConfigurationManager.AppSettings["SubscriptionKey"]);
                HttpPostedFileBase file1 = Request.Files[0];
                HttpPostedFileBase file2 = Request.Files[1];

                if (file1.ContentLength == 0 || file2.ContentLength == 0)
                {
                    return Json(new { error = "Error: It looks like the photo upload din't work..." });
                }

                var faces1 = await client.DetectAsync(file1.InputStream);
                var faces2 = await client.DetectAsync(file2.InputStream);

                if (faces1 == null || faces2 == null)
                {
                    return Json(new { error = "Error: It looks like we can't detect faces in one of these photos..." });
                }
                if (faces1.Count() == 0 || faces2.Count() == 0)
                {
                    return Json(new { error = "Error: It looks like we can't detect faces in one of these photos..." });
                }
                if (faces1.Count() > 1 || faces2.Count() > 1)
                {
                    return Json(new { error = "Error: Each photo must have only one face. Nothing more, nothing less..." });
                }
                var res = await client.VerifyAsync(faces1[0].FaceId, faces2[0].FaceId);
                double score = 0;
                if (res.IsIdentical)
                    score = 100;
                else
                {
                    score = Math.Round((res.Confidence / 0.5) * 100);
                }


                return Json(new { error = "", result = score });
            }
            catch (Exception ex)
            {
                return Json(new { error = "Hmmm... Something unexpected happened. Please come back later." });
            }
        }
    }
}
//*********************************************************  
//  
//TwinsOrNot.Net, https://github.com/matvelloso/twinsornot 
// 
//Copyright (c) Microsoft Corporation 
//All rights reserved.  
// 
// MIT License: 
// Permission is hereby granted, free of charge, to any person obtaining 
// a copy of this software and associated documentation files (the 
// ""Software""), to deal in the Software without restriction, including 
// without limitation the rights to use, copy, modify, merge, publish, 
// distribute, sublicense, and/or sell copies of the Software, and to 
// permit persons to whom the Software is furnished to do so, subject to 
// the following conditions: 


// The above copyright notice and this permission notice shall be 
// included in all copies or substantial portions of the Software. 


// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//  
//*********************************************************  
