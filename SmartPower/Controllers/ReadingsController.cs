using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SmartPower.Data;
using SmartPower.Data.Tables;

namespace SmartPower.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReadingsController : ControllerBase
    {
        private readonly DataContext _context;
        public readonly IHttpClientFactory _clientFactory;

        public ReadingsController(DataContext context, IHttpClientFactory clientFactory)
        {
            _context = context;
            _clientFactory = clientFactory;
        }

        // GET: api/FutureReadings
        [HttpGet]
        public ActionResult GetFutureReading()
        {
            return Ok(_context.Readings.ToList());
        }

        // Create New Reading "GET": api/FutureReadings
        // LineId is prefixed in pMC, example:
        // pMC: 56987, means lineId is 5 and machine code is 6987
        [HttpGet]
        [Route("reading")]
        public async Task<IActionResult> Reading([FromQuery] string pMC, [FromQuery] short pST, [FromQuery] decimal pLength)
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
                Reading reading = _context.Readings.FirstOrDefault(r => r.MachineId == MachineId && r.LineId == LineId && r.Assignment != 2 && r.Assignment!=1);
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
        [Route("length")]
        public IActionResult Length([FromQuery] string PairId, [FromQuery] string MachineId, [FromQuery] int LineId)
        {

            Reading reading = _context.Readings.FirstOrDefault(r => r.PairId == PairId && r.MachineId == MachineId && r.LineId == LineId && r.Status == 0 && r.Assignment!=1);
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
                        return Ok(reading);
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
                            return Ok(reading);
                        }
                        else
                        {
                            return Ok(reading);
                        }
                    }
                }
                else
                {
                    ReadingsLog finished = _context.ReadingsLogs.FirstOrDefault(r => r.PairId == PairId && r.MachineId == MachineId && r.LineId == LineId && r.Status == 0);
                    return Ok(finished);
                }

            }
            else
            {
                return NotFound("reading doesn't exist!");
            }
        }
        [HttpGet]
        [Route("confirmerp")]
        public IActionResult ConfirmERP([FromQuery] string PairId, [FromQuery] string MachineId, [FromQuery] int LineId, [FromQuery] short Flag)
        {
            Reading reading = _context.Readings.FirstOrDefault(r => r.PairId == PairId && r.MachineId == MachineId && r.LineId == LineId && r.Status == 0);
            if (reading.Assignment == 1)
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
                return Ok(FinishedReading);

            }
            else
            {
                return Ok(reading);
            }

        }

        private bool FutureReadingExists(int id)
        {
            return _context.Readings.Any(e => e.Id == id);
        }
    }
}