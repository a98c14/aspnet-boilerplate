using System;
using System.Collections.Generic;

namespace Boiler.Domain.Auth
{
    public class Account
    {
        public int    Id        { get; set; }
        public bool   IsDeleted { get; set; }
        public bool   IsVerified => VerificationDate.HasValue;

        public Role   Role                      { get; set; }
        public string Email                     { get; set; }
        public string PasswordHash              { get; set; }
        public string ResetToken                { get; set; }
        public string VerificationToken         { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }

        public DateTime  CreationDate      { get; set; }
        public DateTime? VerificationDate  { get; set; }
        public DateTime? UpdateDate        { get; set; }
        public DateTime? DeleteDate        { get; set; }
        public DateTime? PasswordResetDate { get; set; }
        public DateTime? ResetTokenExpires { get; set; }

        public bool OwnsToken(string token) => RefreshTokens?.Find(x => x.Token == token) != null;
    }
}
