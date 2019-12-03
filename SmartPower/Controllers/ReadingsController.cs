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
        public IEnumerable<Reading> GetFutureReading()
        {
            return _context.Readings;
        }

        // GET: api/FutureReadings/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFutureReading([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var futureReading = await _context.Readings.FindAsync(id);

            if (futureReading == null)
            {
                return NotFound();
            }

            return Ok(futureReading);
        }

        // PUT: api/FutureReadings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFutureReading([FromRoute] int id, [FromBody] Reading futureReading)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != futureReading.Id)
            {
                return BadRequest();
            }

            _context.Entry(futureReading).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FutureReadingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
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
                return BadRequest(ModelState);
            }

            char[] pMcArray = pMC.ToCharArray();
            int LineId = int.Parse(pMC.Substring(0, 1));
            string MachineId = pMC.Substring(1, pMcArray.Length - 1);
            if (pST == 1)
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
                HttpResponseMessage request = await client.GetAsync("http://5dcd4ed5d795470014e4cf5f.mockapi.io/erp/f2");
                try
                {
                    string json = await request.Content.ReadAsStringAsync();
                    JArray jsonArray = JArray.Parse(json);
                    dynamic erp = JObject.Parse(jsonArray[0].ToString());
                    // assign PairId to Reading r.PairId = response.pairId
                    reading.PairId = erp.PairId;
                    _context.Readings.Add(reading);
                    await _context.SaveChangesAsync();
                    return CreatedAtAction("GetFutureReading", new { id = reading.Id }, reading);
                }
                catch (Exception)
                {
                    return BadRequest("Reading not Created, Couldn't get \"Pair Id\" from ERP");
                }


            }
            else if (pST == 0)
            {
                Reading reading = _context.Readings.FirstOrDefault(r => r.MachineId == MachineId && r.LineId == LineId);
                if (reading != null)
                {
                    //  Check : Get Current Assignment from RFID ?
                    //  Check : Accumulate into one reading at runtime or create new reading for each signal ? 
                    if (reading.Assignment == 0)
                    {
                        // this means machine stopped and a reading
                        // already exists but is not finished yet (due to PowerOutput, StandBy, etc..),
                        // so we update it's already existing length
                        decimal CurrentLength = reading.Length;
                        reading.Length = CurrentLength + pLength;
                        _context.SaveChanges();
                        return NoContent();
                    }
                    else if (reading.Assignment == 1)
                    {
                        // reading is pending and ready to be finished
                        decimal CurrentLength = reading.Length;
                        reading.Length = CurrentLength + pLength;
                        reading.Assignment = 2;
                        reading.EndTime = DateTime.Now;
                        // create a copy for Log table
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
                        // Add the finished reading to Reading Logs table "a table containing finished readings only"
                        _context.ReadingsLogs.Add(FinishedReading);
                        _context.Readings.Add(reading);
                        _context.SaveChanges();
                        return NoContent();
                    }

                }
                else
                {
                    // a finished reading that has no prior record
                    return BadRequest("Reading Never Had a Starting Record!");
                }
                return Ok(reading);
            }
            else
            {
                // machine sent a status other than 1 "starting" or 0 "finished" somehow
                return BadRequest("machine status is not recognized");
            }
        }

        [HttpGet]
        [Route("length")]
        public IActionResult Length([FromQuery] string PairId, [FromQuery] string MachineId, [FromQuery] string LineId, [FromQuery] short Flag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (Flag == 2)
            {
                ReadingsLog reading = _context.ReadingsLogs.FirstOrDefault(r => r.PairId == PairId && r.MachineId == MachineId && r.Assignment == Flag);
                if (reading != null)
                {
                    return Ok(reading);
                }
                else
                {
                    return NotFound("reading doesn't exist!");
                }

            }
            else if (Flag == 2)
            {
                Reading reading = _context.Readings.FirstOrDefault(r => r.PairId == PairId && r.MachineId == MachineId && r.Assignment == Flag);
                if (reading != null)
                {
                    return Ok(reading);
                }
                else
                {
                    return NotFound("reading doesn't exist!");
                }
            }
            else
            {
                return BadRequest("parameter \"flag\" is invalid");
            }
        }

        private bool FutureReadingExists(int id)
        {
            return _context.Readings.Any(e => e.Id == id);
        }
    }
}