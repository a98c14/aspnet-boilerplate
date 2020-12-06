using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace Boiler.Auth.Entities
{
    [Owned]
    public class RefreshToken
    {
        public bool IsExpired => DateTime.UtcNow >= ExpirationDate;
        public bool IsActive  => RevokeDate == null && !IsExpired;

        [Key]
        public int       Id              { get; set; }
        public Account   Account         { get; set; }
        public string    Token           { get; set; }
        public string    CreatedByIp     { get; set; }
        public string    RevokedByIp     { get; set; }
        public string    ReplacedByToken { get; set; }
        public DateTime  ExpirationDate  { get; set; }
        public DateTime  CreationDate    { get; set; }
        public DateTime? RevokeDate      { get; set; }
    }
}
