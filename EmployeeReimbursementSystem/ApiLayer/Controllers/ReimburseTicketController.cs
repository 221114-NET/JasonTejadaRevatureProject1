/*
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// Import necessary layers
using ModelLayer;
using BusinessLayer;

namespace ApiLayer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReimburseTicketController : ControllerBase
    {
        // Dependency injection for ticket service class
        private readonly ITicketService _its;
        public ReimburseTicketController(ITicketService its) => this._its = its;

        [HttpPost("ReimbursementTicket")]
        public ActionResult<ReimburseTicket> ReimbursementTicket(int empId, string reason, int amount, string description) {
            ReimburseTicket ticket = _its.AddTicket(empId, reason, amount, description);
            return Created("path/", ticket);
        }
    }
}