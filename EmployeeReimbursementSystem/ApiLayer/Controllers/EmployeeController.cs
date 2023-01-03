using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using ModelLayer;
using BusinessLayer;

namespace ApiLayer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase {
    // Dependency Injection for Employee Service class and Ticket Service class
    private readonly IEmployeeService _ies;
    private readonly ITicketService _its;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private string _cookieName;
    public EmployeeController(IEmployeeService ies, ITicketService its, IHttpContextAccessor httpContextAccessor) {
        this._ies = ies;
        this._its = its;
        this._httpContextAccessor = httpContextAccessor;
        this._cookieName = "AuthCookie";
    }
        
    [HttpPost("Register")]
    public async Task<ActionResult<Employee>> PostEmployee(Employee e) {
        Employee employee = new Employee();
        try {
            employee = await _ies.PostEmployee(e.email!, e.password!, e.roleID);
        } catch(Exception ex) {
            return StatusCode(500, ex.Message);
        }
        if(employee is null) return StatusCode(400, "Unable to register, invalid input(s).");
        else return StatusCode(201, employee);
    }

    /**
     * TODO, do authentication with sessions + cookie.
     * When user logs in, the API will...
     * Verify the credentials against the database
     * DB creates a temporary user session (in a LoginSession table)
     * API issues a cookie with a sessionId. 
     ** Every request, user sends the cookie for authorization.
     ** Server validates the cookie against the session store (here, a table in database)
     ** User logs out, destroy the session & clear the cookie. (will need a logout func, maybe a timeout too?)
     */
    [HttpPost("LoginEmployee")]
    public async Task<ActionResult<Employee>> LoginEmployee(Employee e) {
        string sessionId = null!;
        try { 
            // TODO sessionId is a guid, look into System.Security.Cryptography to generate better ids
            sessionId = await _ies.LoginEmployee(e.email!, e.password!);
            if(sessionId is null) return StatusCode(400, "Unable to login, invalid input(s).");
            CookieOptions options = new CookieOptions();
            options.Expires = DateTime.Now.AddMinutes(15);
            options.Path = "/"; // ? what does this do
            options.Secure = true; // Ensure cookie is properly secured using SSL/TLS encryption(?)
            _httpContextAccessor.HttpContext!.Response.Cookies.Append(_cookieName, sessionId, options);
        } catch(Exception ex) {
            return StatusCode(500, ex.Message);
        }
        return StatusCode(200, sessionId);
    }

    // TODO Add authorization
    [HttpPut("ChangePassword")]
    public async Task<ActionResult<Employee>> EditEmployee(Employee e, string oldPassword) {
        Employee employee = new Employee();
        try {
            employee = await _ies.EditEmployee(e.id, oldPassword, e.password!);
        } catch(Exception ex) {
            return StatusCode(500, ex.Message);
        }
        if(employee is null) return StatusCode(400, "Unable to update password, invalid input(s).");
        else return StatusCode(200, employee);
    }

    // TODO Add authorization
    [HttpPut("ChangeEmail")]
    public async Task<ActionResult<Employee>> EditEmployee(Employee e) {
        Employee employee = new Employee();
        try {
            employee = await _ies.EditEmployee(e.id, e.email!);
        } catch(Exception ex) {
            return StatusCode(500, ex.Message);
        }
        if(employee is null) return StatusCode(400, "Unable to update email, invalid input(s).");
        else return StatusCode(200, employee);
    }

    // TODO Add authorization
    [HttpPut("ChangeRole")]
    public async Task<ActionResult<Employee>> EditEmployee(int managerId, int targetId, int newRoleId) {
        Employee employee = new Employee();
        try {
            employee = await _ies.EditEmployee(managerId, targetId, newRoleId);
        } catch(Exception ex) {
            return StatusCode(500, ex.Message);
        }
        if(employee is null) return StatusCode(400, "Unable to update role, invalid input(s).");
        else return StatusCode(200, employee);
    }

    [HttpGet("EmployeeTickets")]
    public async Task<ActionResult<List<ReimburseTicket>>> EmployeeTickets(int employeeId) {
        List<ReimburseTicket> tickets = new List<ReimburseTicket>();
        var cookie = Request.Cookies[_cookieName];
        try {
            if(cookie is null) return StatusCode(401, "Error: Invalid cookies");
            tickets = await _its.GetEmployeeTickets(employeeId);
        } catch(Exception ex) {
            return StatusCode(500, ex.Message);
        }
        if(tickets is null) return StatusCode(401, "Unable to retrieve tickets, invalid input.");
        else { 
            // Extend time on cookie
            _httpContextAccessor.HttpContext!.Response.Cookies.Delete(cookie);
            CookieOptions options = new CookieOptions();
            options.Expires = DateTime.Now.AddMinutes(15);
            options.Path = "/"; // Make cookie available to all parts of the system
            options.Secure = true; // Ensure cookie is properly secured using SSL/TLS encryption(?)
            _httpContextAccessor.HttpContext!.Response.Cookies.Append(_cookieName, cookie, options);

            return StatusCode(200, tickets);
        }
    }

    [HttpGet("EmployeeTicketsByStatus")]
    public async Task<ActionResult<List<ReimburseTicket>>> EmployeeTickets(int employeeId, int status) {
        List<ReimburseTicket> tickets = new List<ReimburseTicket>();
        try {
            tickets = await _its.GetEmployeeTickets(employeeId, status);
        } catch(Exception ex) {
            return StatusCode(500, ex.Message);
        }
        if(tickets is null) return StatusCode(400, "Unable to retrieve tickets, invalid input.");
        else return StatusCode(200, tickets);
    }
}