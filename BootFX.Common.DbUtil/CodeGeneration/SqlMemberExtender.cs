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
    internal static class SqlMemberExtender
    {
        internal static MemberAttributes GetMemberAttributes(ColumnModifiers modifiers, bool isVirtual, EntityGenerationContext context)
        {
            switch (modifiers)
            {
                case ColumnModifiers.Internal:
                    if (!(isVirtual))
                        return MemberAttributes.Assembly | MemberAttributes.Final;
                    else
                        return MemberAttributes.Assembly;

                case ColumnModifiers.Private:
                    if (!(isVirtual))
                        return MemberAttributes.Private | MemberAttributes.Final;
                    else
                        return MemberAttributes.Private;

                case ColumnModifiers.Protected:
                    if (!(isVirtual))
                        return MemberAttributes.Family | MemberAttributes.Final;
                    else
                        return MemberAttributes.Family;

                case ColumnModifiers.ProtectedInternal:
                    if (!(isVirtual))
                        return MemberAttributes.FamilyOrAssembly | MemberAttributes.Final;
                    else
                        return MemberAttributes.FamilyOrAssembly;

                case ColumnModifiers.Public:
                    if (!(isVirtual))
                        return MemberAttributes.Public | MemberAttributes.Final;
                    else
                        return MemberAttributes.Public;

                default:
                    throw new NotSupportedException(string.Format("Cannot handle '{0}'.", modifiers));
            }
        }

        internal static TypeAttributes GetTypeAttributes(TableModifiers modifiers, bool isAbstract, EntityGenerationContext context)
        {
            // what?
            switch (modifiers)
            {
                case TableModifiers.Internal:
                    if (isAbstract)
                        return TypeAttributes.NestedAssembly | TypeAttributes.Abstract;
                    else
                        return TypeAttributes.NestedAssembly;

                case TableModifiers.Public:
                    if (isAbstract)
                        return TypeAttributes.Public | TypeAttributes.Abstract;
                    else
                        return TypeAttributes.Public;

                default:
                    throw new NotSupportedException(string.Format("Cannot handle '{0}'.", modifiers));
            }
        }
    }
}
