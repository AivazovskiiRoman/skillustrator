using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace app.Migrations
{
    public partial class PopulateSkillLevelAndTimePeriod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("insert into \"SkillLevel\" (\"Code\", \"Description\") values ('BEGINNER', 'Beginner')");
            migrationBuilder.Sql("insert into \"SkillLevel\" (\"Code\", \"Description\") values ('INTERMEDIATE', 'Intermediate')");
            migrationBuilder.Sql("insert into \"SkillLevel\" (\"Code\", \"Description\") values ('EXPERT', 'Expert')");
                                                                                     
            migrationBuilder.Sql("insert into \"TimePeriod\" (\"Code\", \"Description\") values ('LESS_ONE_MONTH', 'Less Than One Month')");
            migrationBuilder.Sql("insert into \"TimePeriod\" (\"Code\", \"Description\") values ('ONE_TO_SIX_MONTHS', 'One to Six Months')");
            migrationBuilder.Sql("insert into \"TimePeriod\" (\"Code\", \"Description\") values ('SIX_TO_TWELVE_MONTHS', 'Six to Twelve Months')");
            migrationBuilder.Sql("insert into \"TimePeriod\" (\"Code\", \"Description\") values ('ONE_TO_TWO_YEARS', 'One to Two Years')");
            migrationBuilder.Sql("insert into \"TimePeriod\" (\"Code\", \"Description\") values ('MORE_TWO_YEARS', 'More Than Two Years')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("truncate table \"SkillLevel\"");
            migrationBuilder.Sql("truncate table \"TimePeriod\"");
        }
    }
}
