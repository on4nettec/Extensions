using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace On4Net.Extensions.Data.Schema;

public class Message
{
    public long Id { get; set; }

    public Guid MessageId { get; set; }

    public string QueueUrl { get; set; }

    public string GroupId { get; set; }

    public string DeduplicationId { get; set; }

    public string Payload { get; set; }

    public DateTime CreatedAt { get; set; }
}
