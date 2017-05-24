using System.Collections.Generic;

namespace Skillustrator.Api.Entities
{
    public class Person : EntityBase
    {
        public Person()
        {

        }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public ICollection<PersonSkill> Skills { get; set; }

        public int? OrganizationId { get; set; }

        public Organization Organization { get; set; }
    }
}