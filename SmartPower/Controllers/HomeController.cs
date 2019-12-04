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
            var model = _context.ReadingsLogs.OrderByDescending(o => o.StartTime).ToList();
            return View(model);
        }


        //public async Task<string> Update()
        //{
        //    var query = HttpContext.Request.Query;
        //    var values = query.Where(o => o.Key.StartsWith("p")).ToDictionary(o => o.Key);
        //    Console.WriteLine("Recieved");
        //    Reading reading = new Reading()
        //    {

        //        MachineCode = Convert.ToString(values["pMC"].Value),
        //        Length = Convert.ToDecimal(values["pLength"].Value),

        //        status = Convert.ToInt32(values["pST"].Value),

        //        time = DateTime.Now,
        //    };
        //    //machine starting 
        //    if (reading.Length == 0 && reading.status == 1)
        //    {
        //        Console.WriteLine("Status is 1");

        //        var jobOrders = DB.jobOrders.Where(j => j.MachineCode == Convert.ToString(values["pMC"].Value)).ToList();

        //        if (jobOrders.Any())
        //        {
        //            Console.WriteLine("job orders already exist");
        //            reading.JobOrders = jobOrders;
        //            DB.SaveChanges();
        //        }
        //        else
        //        {
        //            Console.WriteLine("getting and adding job orders");
        //            AddJobOrder(getJob(Convert.ToString(values["pMC"].Value)),reading);
        //            reading.JobOrders = jobOrders;
        //        }
        //    }
        //    else if (reading.Length != 0 && reading.status == 0)
        //    {
        //        Console.WriteLine("Status is 0 complete");
        //        var jobOrders = DB.jobOrders.Where(j => j.MachineCode == Convert.ToString(values["pMC"].Value) && j.TotalLength == 0);

        //        reading.status = 0;
        //        await jobOrders.ForEachAsync(j =>
        //        {
        //            j.EndDate = DateTime.Now;
        //            j.TotalLength = reading.Length;
        //        });
        //        reading.JobOrders = jobOrders.ToList();
        //        DB.jobOrders.UpdateRange(jobOrders);
        //        DB.SaveChanges();

        //    }//else if(encoder.Length != 0 && encoder.status == 2)
        //     //{

        //    //}


        //    DB.Reading.Add(reading);
        //    //DB.re.Add(encoder);
        //    DB.SaveChanges();
        //    return $"ok";
        //}
        public IActionResult Index()
        {
            return RedirectToAction("Data");
        }
        // GET: api/FutureReadings
        [HttpGet]
        public IEnumerable<Reading> GetReadings()
        {
            return _context.Readings;
        }

        // Create New Reading "GET": api/FutureReadings
        // LineId is prefixed in pMC, example:
        // pMC: 56987, means lineId is 5 and machine code is 6987
        [HttpGet]
        public async Task<IActionResult> Update([FromQuery] string pMC, [FromQuery] short pST, [FromQuery] decimal pLength)
        {
            // return Ok((machinecode:  pMC, status:  pST, length: pLength));
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            char[] pMcArray = pMC.ToCharArray();
            int LineId = int.Parse(pMC.Substring(0, 1));
            string MachineId = pMC.Substring(1, pMcArray.Length - 1);
            if (pST == 1 && pLength == 0)
            {
                // Starting

                Reading reading = new Reading
                {
                    Assignment = 0, //free
                    StartTime = DateTime.Now,
                    Length = pLength,
                    MachineId = MachineId,
                    LineId = LineId,
                    Status = pST,

                };
                // request (await) pairId from erp by sending (MachineCode, LineId)
                HttpClient client = _clientFactory.CreateClient();
                HttpResponseMessage request = await client.GetAsync("http://5dcd4ed5d795470014e4cf5f.mockapi.io/erp/PairID");
                try
                {
                    string json = await request.Content.ReadAsStringAsync();
                    JArray jsonArray = JArray.Parse(json);
                    dynamic erp = JObject.Parse(jsonArray[0].ToString());
                    // assign PairId to Reading r.PairId = response.pairId
                    reading.PairId = erp.PairId;
                    _context.Readings.Add(reading);
                    await _context.SaveChangesAsync();
                    return Ok("ok");
                }
                catch (Exception)
                {
                    return BadRequest();
                }


            }
            else if (pST == 0 && pLength != 0)
            {
                Reading reading = _context.Readings.FirstOrDefault(r => r.MachineId == MachineId && r.LineId == LineId && r.Assignment != 2 && r.Assignment != 1);
                if (reading != null)
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
                    return Ok("ok");
                }
                else
                {
                    // a finished reading that has no prior record
                    return BadRequest();
                }
            }
            else
            {
                // machine sent a status other than 1 "starting" or 0 "finished" somehow
                return BadRequest();
            }
        }

        [HttpGet]
        public IActionResult ConfirmFinished([FromQuery] string PairId, [FromQuery] string MachineId, [FromQuery] int LineId)
        {

            Reading reading = _context.Readings.FirstOrDefault(r => r.PairId == PairId && r.MachineId == MachineId && r.LineId == LineId && r.Status == 0 && r.Assignment != 1);
            if (reading != null)
            {
                ReadingsLog log = _context.ReadingsLogs.FirstOrDefault(r => r.PairId == PairId && r.MachineId == MachineId && r.LineId == LineId && r.Status == 0);
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
                    ReadingsLog finished = _context.ReadingsLogs.FirstOrDefault(r => r.PairId == PairId && r.MachineId == MachineId && r.LineId == LineId && r.Status == 0);
                    dynamic response = new { length = finished.Length, Message = "Success", statusCode = "OK" };
                    dynamic list = new List<dynamic>() { response };

                    return Ok(list);
                }

            }
            else
            {
                return NotFound("reading doesn't exist!");
            }
        }
        [HttpGet]
        public IActionResult ConfirmERP([FromQuery] string PairId, [FromQuery] string MachineId, [FromQuery] int LineId, [FromQuery] short Flag)
        {
            Reading reading = _context.Readings.FirstOrDefault(r => r.PairId == PairId && r.MachineId == MachineId && r.LineId == LineId && r.Status == 0);
            if (reading.Assignment == 1 || reading.Assignment == 0) // will he confirm after reading or confirm instantly
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

                _context.ReadingsLogs.Add(FinishedReading);
                _context.Readings.Remove(reading);
                _context.SaveChanges();
                ReadingsLog finished = _context.ReadingsLogs.FirstOrDefault(r => r.PairId == PairId && r.MachineId == MachineId && r.LineId == LineId && r.Status == 0);
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


        //takes the list of job orders and assigns each one to the machine (machine code)
        //public void AddJobOrder(ICollection<string> Jobs, Reading reading)
        //{
        //    var JobList = Jobs.ToList();
        //    DateTime date = DateTime.Now;
        //    Console.WriteLine("About to print JOBS before ADDING");
        //    JobList.ForEach(j =>
        //    {
        //        JobOrder jobOrder = new JobOrder()
        //        {

        //            JobOrderId = j,
        //            StartDate = date,
        //            EndDate = null,
        //            MachineCode = reading.MachineCode,
        //            TotalLength = 0
        //        };
        //        DB.jobOrders.Add(jobOrder);
        //    });
        //    DB.SaveChanges();

        //    //DB.jobOrders.AddRange(jobOrders);
        //    //readings.ForEachAsync(r => r.jobOrders = jobOrders);
        //    //DB.SaveChangesAsync();


        //}

        //public void UpdateJobOrder(ICollection <JobOrder> orders, decimal length)
        //{
        //    JobOrder jobOrder = DB.jobOrders.FirstOrDefault(j => j.JobOrderId == jobId);
        //    jobOrder.EndDate = DateTime.Now;
        //    jobOrder.TotalLength = length;
        //    DB.jobOrders.Update(jobOrder);
        //    DB.SaveChanges();
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
