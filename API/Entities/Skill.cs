using System.Collections.Generic;

namespace Skillustrator.Api.Entities
{
    public class Skill : EntityBase
    {
        public Skill()
        {

        }
        public string Name { get; set; }

        public ICollection<SkillTag> Tags { get; set; }
    }
}