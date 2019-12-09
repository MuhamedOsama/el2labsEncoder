using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using LG.Models;

using Microsoft.Extensions.Configuration;
using SmartPower.Data.Tables;
using Microsoft.EntityFrameworkCore;
using SmartPower.Data;
using System.Net;
using RestSharp;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

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
        public IActionResult Data()
        {
            var model = _context.LengthReadingsLogs.OrderByDescending(o => o.StartTime).ToList();
            return View(model);
        }


        public IActionResult Index()
        {
            return RedirectToAction("Data");
        }
        // GET: api/FutureReadings
        [HttpGet]
        public IEnumerable<Reading> GetReadings()
        {
            return _context.LengthReadings;
        }

        // Create New Reading "GET": api/FutureReadings
        // LineId is prefixed in pMC, example:
        // pMC: 56987, means lineId is 5 and machine code is 6987
        [HttpGet]
        public async Task<string> Update([FromQuery] string pMC, [FromQuery] short pST, [FromQuery] decimal pLength)
        {
            char[] pMcArray = pMC.ToCharArray();
            int LineId = int.Parse(pMC.Substring(0, 1));
            string MachineId = pMC.Substring(1, pMcArray.Length - 1);
            
            if (pST == 0 && pLength != 0)
            {
                Reading reading = _context.LengthReadings.FirstOrDefault(r => r.MachineId == MachineId && r.LineId == LineId && r.Assignment != 2 && r.Assignment != 1);
                if (reading!=null)
                {
                    //  Check : Get Current Assignment from RFID ?
                    //  Check : Accumulate into one reading at runtime or create new reading for each signal ? 

                    // this means machine stopped and a reading
                    // already exists but is not finished yet (due to PowerOutput, StandBy, etc..),
                    // so we update it's already existing length
                    decimal CurrentLength = reading.Length;
                    reading.Status = pST;
                    reading.EndTime = DateTime.Now;
                    reading.Length = CurrentLength + pLength;
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
                        //Console.WriteLine("EEEEEEEEEEE" + expiration.Minute);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", " " + (String)token);
                        client.DefaultRequestHeaders.Add("uid", "1");
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
                    catch (Exception e)
                    {
                        return e.Message;
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
                ReadingsLog log = _context.LengthReadingsLogs.FirstOrDefault(r => r.PairId == PairId && r.MachineId == MachineId && r.LineId == LineId && r.Status == 0);
                if (log == null)
                {
                    if (reading.Assignment == 0)
                    {
                        reading.Assignment = 1;
                        reading.LastRequest = DateTime.Now;
                        _context.SaveChanges();
                        dynamic response = new { length = reading.Length, Message = "Success", statusCode = "OK" };
                        dynamic list = new List<dynamic>() { response};

                        return Ok(list);
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
                            dynamic list = new List<dynamic>() { response };

                            return Ok(list);
                        }
                        else
                        {
                            dynamic response = new { length = reading.Length, Message = "Success", statusCode = "OK" };
                            dynamic list = new List<dynamic>() { response };

                            return Ok(list);
                        }
                    }
                }
                else
                {
                    ReadingsLog finished = _context.LengthReadingsLogs.FirstOrDefault(r => r.PairId == PairId && r.MachineId == MachineId && r.LineId == LineId && r.Status == 0);
                    dynamic response = new { length = finished.Length, Message = "Success", statusCode = "OK" };
                    dynamic list = new List<dynamic>() { response };

                    return Ok(list);
                }

            }
            else
            {
                dynamic response = new { Message = "Failed", statusCode = "ERROR" };
                dynamic list = new List<dynamic>() { response };

                return Ok(list);
            }
        }
        [HttpGet]
        public IActionResult ConfirmFinished([FromQuery] string PairId, [FromQuery] string MachineId, [FromQuery] int LineId, [FromQuery] short Flag)
        {
            Reading reading = _context.LengthReadings.FirstOrDefault(r => r.PairId == PairId && r.MachineId == MachineId && r.LineId == LineId && r.Status == 0);
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
                dynamic response = new {Message = "Success", statusCode = "OK"};
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


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
