//Copyright © 2014 Sony Computer Entertainment America LLC. See License.txt.

using System;

namespace Sce.Atf.Applications.WebServices
{
    /// <summary>
    /// This attribute can be placed on an assembly (usually in the AssemblyInfo file) to
    /// indicate the identifier that is used for mapping to a SourceForge project for
    /// version checking and bug submission. (If this attribute is not present, the
    /// AssemblyTitle attribute is used instead.)
    ///
    /// See
    ///   http://wiki.ship.scea.com/confluence/display/SUPPORT/Bug+Submit+and+Version+Check+services
    /// for more information.</summary>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
    public class ProjectMappingAttribute : Attribute
    {
        /// <summary>
        /// Constructor</summary>
        /// <param name="mapping">Name that is used to identify the file package and bug database
        /// in SourceForge</param>
        public ProjectMappingAttribute(string mapping)
        {
            m_mapping = mapping;
        }

        /// <summary>
        /// Get or set the name that is used to identify the file package and bug database in SourceForge.
        /// This name is not necessarily the package name; it often starts with "com.scea"
        /// (for example, "com.scea.screamtool").
        /// See http://wiki.ship.scea.com/confluence/display/SUPPORT/Bug+Submit+and+Version+Check+services </summary>
        public string Mapping
        {
            get { return m_mapping; }
            set { m_mapping = value; }
        }

        private string m_mapping;
    }
}
