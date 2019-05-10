using BootFX.Common.Data.Schema;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BootFX.Common.CodeGeneration
{
    internal static class SqlColumnExtender
    {
        internal static MemberAttributes GetMemberAttributes(this SqlColumn column, bool isVirtual, EntityGenerationContext context)
        {
            return SqlMemberExtender.GetMemberAttributes(column.Modifiers, isVirtual, context);
        }
    }
}
