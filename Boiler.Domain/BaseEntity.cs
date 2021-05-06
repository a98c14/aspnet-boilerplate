using System;

namespace Boiler.Domain
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public string ApiKey { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
