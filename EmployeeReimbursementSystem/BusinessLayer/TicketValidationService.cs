using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ModelLayer;
using RepositoryLayer;

namespace BusinessLayer;
public interface ITicketValidationService {
    public bool ValidReason(string reason);
    public bool ValidAmount(double amount);
    public bool ValidDescription(string description);
    public bool ValidTicket(string reason, double amount, string description);
    public bool isTicket(string ticketId);
    public bool ValidStatusChange(int managerId, string ticketId);
}

public class TicketValidationService : ITicketValidationService {
    private readonly ITicketRepository _itr;
    public TicketValidationService(ITicketRepository itr) => this._itr = itr;
    #region // Ticket Input Validation
    public bool ValidTicket(string reason, double amount, string desc) => ValidReason(reason) && ValidAmount(amount) && ValidDescription(desc);
    public bool ValidReason(string reason) => reason.Length > 1;
    public bool ValidDescription(string description) => description.Length > 1;
    public bool ValidAmount(double amount) => (amount > 0 && amount < 10000);
    #endregion

    public bool isTicket(string ticketId) {
        if(_itr.GetTicket(ticketId) is null) return false;
        else return true;
    }

    public bool ValidStatusChange(int managerId, string ticketId) {
        ReimburseTicket tmp = _itr.GetTicket(ticketId);
        if(tmp.employeeID == managerId || tmp is null || tmp.status != 0) return false;
        return true;
    }
}