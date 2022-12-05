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

        [HttpGet("PendingTickets")]
        public ActionResult<List<ReimburseTicket>> PendingTickets(int empId) {
            List<ReimburseTicket> pendingTickets = _its.GetPendingTickets(empId);
            return Created("path/", pendingTickets);
        }

        [HttpPut("Approve")]
        public ActionResult<ReimburseTicket> Approve(int empId, int ticketId) {
            ReimburseTicket approvedTicket = _its.ApproveTicket(empId, ticketId);
            return Created("path/", approvedTicket);
        }

        [HttpPut("Deny")]
        public ActionResult<ReimburseTicket> Deny(int empId, int ticketId) {
            ReimburseTicket deniedTicket = _its.DenyTicket(empId, ticketId);
            return Created("path/", deniedTicket);
        }

        [HttpGet("EmployeeTickets")]
        public ActionResult<List<ReimburseTicket>> EmployeeTickets(int empId) {
            List<ReimburseTicket> employeeTickets = _its.GetEmployeeTickets(empId);
            return Created("path/", employeeTickets);
        }

        [HttpGet("EmployeeTicketsByStatus")]
        public ActionResult<List<ReimburseTicket>> EmployeeTickets(int empId, int status) {
            List<ReimburseTicket> employeeTickets = _its.GetEmployeeTickets(empId, status);
            return Created("path/", employeeTickets);
        }
    }
}