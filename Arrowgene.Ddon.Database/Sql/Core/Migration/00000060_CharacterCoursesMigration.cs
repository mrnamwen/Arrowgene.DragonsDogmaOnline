using System.Data.Common;

namespace Arrowgene.Ddon.Database.Sql.Core.Migration
{
    public class CharacterCoursesMigration(DatabaseSetting databaseSetting) : IMigrationStrategy
    {
        public uint From => 59;
        public uint To => 60;

        public bool Migrate(IDatabase db, DbConnection conn)
        {
            string adaptedSchema = DdonDatabaseBuilder.GetAdaptedSchema(databaseSetting, "Script/migration_character_courses.sql");
            db.Execute(conn, adaptedSchema, true);
            return true;
        }
    }
}
