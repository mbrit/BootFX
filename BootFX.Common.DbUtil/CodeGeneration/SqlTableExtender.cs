using BootFX.Common.Data.Schema;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BootFX.Common.CodeGeneration
{
    internal static class SqlTableExtender
    {
        /// <summary>
        /// Gets the TypeAttributes for the property Modifiers
        /// </summary>
        /// <returns></returns>
        internal static MemberAttributes GetMemberAttributes(this SqlTable table, bool isVirtual, EntityGenerationContext context)
        {
            // to get member attributes, we need to take the modifiers and return something else...
            switch (table.Modifiers)
            {
                case TableModifiers.Public:
                    return SqlMemberExtender.GetMemberAttributes(ColumnModifiers.Public, isVirtual, context);

                case TableModifiers.Internal:
                    return SqlMemberExtender.GetMemberAttributes(ColumnModifiers.Internal, isVirtual, context);

                default:
                    throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", table.Modifiers, table.Modifiers.GetType()));
            }
        }

        public static TypeAttributes GetTypeAttributes(this SqlTable table, bool isVirtual, EntityGenerationContext context)
        {
            return SqlMemberExtender.GetTypeAttributes(table.Modifiers, isVirtual, context);
        }
    }
}
