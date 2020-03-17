using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using LG.Models;
using SmartPower.Data.Tables;
using SmartPower.Data;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using Encoder.Data.Tables;

namespace SmartPower.Controllers
{
    public class HomeController : Controller
    {

        private readonly DataContext _context;
        public readonly IHttpClientFactory _clientFactory;
        public HomeController(DataContext dataContext, IHttpClientFactory clientFactory)
        {

            _context = dataContext;
            _clientFactory = clientFactory;
        }
        public IActionResult PendingData()
        {
            var model = _context.LengthReadings.OrderByDescending(o => o.StartTime).ToList();
            return View(model);
        }
        public IActionResult Data()
        {
            var model = _context.LengthReadingsLogs.OrderByDescending(o => o.StartTime).ToList();
            return View(model);
        }

        [HttpGet]
        public ActionResult Readings()
        {
            return Ok(_context.LengthReadings.ToList());
        }

        public IActionResult Index()
        {
            return RedirectToAction("Data");
        }

        // Create New Reading "GET": api/FutureReadings
        // LineId is prefixed in pMC, example:
        // pMC: 56987, means lineId is 5 and machine code is 6987
        [HttpGet]
        public async Task<string> Update([FromQuery] string pMC, [FromQuery] short pST, [FromQuery] decimal pLength)
        {

            int LineId = int.Parse(pMC[pMC.Length-1].ToString());
            string MachineId = pMC.Substring(0, pMC.Length - 2);
            
            if (pST == 0 && pLength > 50)
            {
                GenericReading genericReading = new GenericReading
                {
                    Length = pLength,
                    MachineId = MachineId,
                    LineId = LineId,
                    TimeStamp = DateTime.Now,
                };
                _context.GenericReadings.Add(genericReading);
                await _context.SaveChangesAsync();
                Reading reading = _context.LengthReadings.FirstOrDefault(r => r.MachineId == MachineId && r.LineId == LineId && r.Assignment != 2 && r.Assignment != 1);
                if (reading!=null)
                {
                    // this means machine stopped and a reading
                    // already exists but is not finished yet (due to PowerOutput, StandBy, etc..),
                    // so we update it's already existing length
                    
                    try
                    {
                        decimal CurrentLength = reading.Length;
                        reading.Status = pST;
                        reading.EndTime = DateTime.Now;
                        reading.Length = CurrentLength + pLength;
                        HttpClient client = _clientFactory.CreateClient();
                        var data = new Dictionary<string, string>();
                        data.Add("grant_type", "password");
                        data.Add("username", "sbta");
                        data.Add("password", "r4e3w2q1");
                        var content = new FormUrlEncodedContent(data);
                        HttpResponseMessage tokenRequest = await client.PostAsync("http://10.1.10.56:9090/api/v1/user/accessToken", content);
                        //tokenRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
                        JObject TokenResponse = JObject.Parse(await tokenRequest.Content.ReadAsStringAsync());
                        dynamic token = TokenResponse["access_token"];
                        DateTime expiration = (DateTime)TokenResponse[".expires"];
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", " " + (String)token);
                        client.DefaultRequestHeaders.Add("uid", MachineId);
                        var json = await client.GetAsync("http://10.1.10.56:9090/api/v1/pairingout/GetPairingSummary");


                        var PairIdJson = await json.Content.ReadAsStringAsync();
                        dynamic PairIdData = JObject.Parse(PairIdJson);
                        if (PairIdData.statusCode == "OK")
                        {
                            //assign PairId to Reading r.PairId = response.pairId
                            reading.PairId = PairIdData.pairingSummary.pairId;
                            genericReading.PairId = PairIdData.pairingSummary.pairId;
                            await _context.SaveChangesAsync();
                        }

                        return $"ok";
                    }
                    catch (Exception)
                    {
                        return $"ok";
                    }
                    _context.SaveChanges();
                    return $"ok";
                }
                else
                {
                    Reading newReading = new Reading
                    {
                        Assignment = 0, //free
                        StartTime = DateTime.Now,
                        Length = pLength,
                        MachineId = MachineId,
                        LineId = LineId,
                        EndTime = DateTime.Now,
                        Status = pST,

                    };
                    
                    //HttpResponseMessage request = await client.PostAsJsonAsync(Url, new { MachineId, LineId });
                    try
                    {   
                        HttpClient client = _clientFactory.CreateClient();
                        var data = new Dictionary<string, string>();
                        data.Add("grant_type", "password");
                        data.Add("username", "sbta");
                        data.Add("password", "r4e3w2q1");
                        var content = new FormUrlEncodedContent(data);
                        HttpResponseMessage tokenRequest = await client.PostAsync("http://10.1.10.56:9090/api/v1/user/accessToken", content);
                        //tokenRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
                        JObject TokenResponse = JObject.Parse(await tokenRequest.Content.ReadAsStringAsync());
                        dynamic token = TokenResponse["access_token"];
                        DateTime expiration = (DateTime)TokenResponse[".expires"];
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", " " + (String)token);
                        client.DefaultRequestHeaders.Add("uid", MachineId);
                        var json = await client.GetAsync("http://10.1.10.56:9090/api/v1/pairingout/GetPairingSummary");
                        

                        var PairIdJson = await json.Content.ReadAsStringAsync();
                        dynamic PairIdData = JObject.Parse(PairIdJson);
                        if(PairIdData.statusCode == "OK")
                        {
                            //assign PairId to Reading r.PairId = response.pairId
                            newReading.PairId = PairIdData.pairingSummary.pairId;
                            _context.LengthReadings.Add(newReading);
                            await _context.SaveChangesAsync();
                        }
                        
                        return $"ok";
                    }
                    catch (Exception)
                    {
                        return $"ok";
                    }
                }
            }
            else
            {
                // machine sent a status other than 0 "finished" somehow
                return $"ok";
            }
        }

        [HttpGet]
        public IActionResult GetLength([FromQuery] string PairId, [FromQuery] string MachineId, [FromQuery] int LineId)
        {

            Reading reading = _context.LengthReadings.FirstOrDefault(r => r.PairId == PairId && r.MachineId == MachineId && r.LineId == LineId && r.Status == 0 && r.Assignment != 2);
            if (reading != null)
            {
               
                    if (reading.Assignment == 0)
                    {
                        reading.Assignment = 1;
                        reading.LastRequest = DateTime.Now;
                        _context.SaveChanges();
                        dynamic response = new { length = reading.Length, Message = "Success", statusCode = "OK" };
                        return Ok(response);
                        //reading.Assignment = 1; //pending
                        //return Ok(reading);
                    }
                    else
                    {
                        TimeSpan diff = DateTime.Now - reading.LastRequest;
                        if (diff.TotalMinutes > 15)
                        {
                            reading.Assignment = 0;
                            _context.SaveChanges();
                            dynamic response = new { length = reading.Length, Message = "Success", statusCode = "OK" };
                            return Ok(response);
                        }
                        else
                        {
                            dynamic response = new { length = reading.Length, Message = "Success", statusCode = "OK" };
                            return Ok(response);
                        }
                    }

            }
            else
            {
                dynamic response = new { Message = "Failed", statusCode = "ERROR" };
                return Ok(response);
            }
        }
        [HttpGet]
        public IActionResult ConfirmFinished([FromQuery] string PairId, [FromQuery] string MachineId, [FromQuery] int LineId)
        {
            Reading reading = _context.LengthReadings.FirstOrDefault(r => r.PairId == PairId && r.MachineId == MachineId && r.LineId == LineId && r.Status == 0);
            if (reading != null)
            {


                if (reading.Assignment == 1) // will he confirm after reading or confirm instantly
                {
                    reading.Assignment = 2;
                    reading.LastRequest = DateTime.Now;
                    ReadingsLog FinishedReading = new ReadingsLog
                    {
                        MachineId = reading.MachineId,
                        Length = reading.Length,
                        Status = reading.Status,
                        LineId = reading.LineId,
                        PairId = reading.PairId,
                        StartTime = reading.StartTime,
                        EndTime = reading.EndTime,
                        Assignment = reading.Assignment
                    };

                    _context.LengthReadingsLogs.Add(FinishedReading);
                    _context.LengthReadings.Remove(reading);
                    _context.SaveChanges();
                    ReadingsLog finished = _context.LengthReadingsLogs.FirstOrDefault(r => r.PairId == PairId && r.MachineId == MachineId && r.LineId == LineId && r.Status == 0);
                    dynamic response = new { Message = "Success", statusCode = "OK" };
                    dynamic list = new List<dynamic>() { response };
                    return Ok(list);

                }
                else
                {
                    dynamic response = new { Message = "Success", statusCode = "OK" };
                    dynamic list = new List<dynamic>() { response };
                    return Ok(list);
                }
            }
            else
            {
                dynamic response = new { Message = "Failed", statusCode = "ERROR" };
                return Ok(response);
            }

        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
