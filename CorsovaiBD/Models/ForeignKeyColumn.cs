using System;
using MySqlConnector;
using System.Collections.Generic;
namespace CorsovaiBD.Models
{
   

    public class ForeignKeyColumn
    {
        public string ColumnName { get; set; }
        public string ReferencedTableName { get; set; }
        public string ReferencedColumnName { get; set; }
        public string AllCollums { get; set; }
        public ForeignKeyColumn(string columnName, string referencedTableName, string referencedColumnName)
        {
            ColumnName = columnName;
            ReferencedTableName = referencedTableName;
            ReferencedColumnName = referencedColumnName;
        }
      
    }

    

}

