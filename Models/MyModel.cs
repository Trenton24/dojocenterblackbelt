using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace exam.Models
{
    public class Users
    {
       [Key]
       public int UserId {get;set;}
       
       [Required]
       [MinLength(2)]
       public string FirstName {get;set;}

       [Required]
       [MinLength(2)]
       public string LastName {get;set;}

       [Required]
       [EmailAddress]
       public string Email {get;set;}

       [DataType(DataType.Password)]
       [Required]
       [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]
       public string Password {get;set;}
       public DateTime CreatedAt {get;set;} = DateTime.Now;
       public DateTime UpdatedAt {get;set;} = DateTime.Now;
       public List<Event> CreatedEvent {get;set;}
       public List<RSVP> Attending {get;set;}

       [NotMapped]
       [Compare("Password")]
       [DataType(DataType.Password)]
       public string Confirm {get;set;}
    }

    public class RSVP
    {
        [Key]
        public int RSVPId {get;set;}
        public int EventId {get;set;}
        public int UserId {get;set;}
        public Users Users {get;set;}
        public Event Event {get;set;}
    }

    public class Event
    {
        [Key]
        public int EventId {get;set;}
        public int UserId {get;set;}
        public List<RSVP> Attending {get;set;}
        [Required]
        [MinLength(4)]
        public string Name {get;set;}
        public int Duration {get;set;}
        public string Time {get;set;}
        [Required]
        [Range(typeof(DateTime), "6/21/2019", "12/31/4000")]
        public DateTime Date {get;set;}
        [Required]
        [MinLength(10)]
        public string Description {get;set;}
        public Users Planner {get;set;}

    }
}