using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace UnizenBot.Meta
{
    /// <summary>
    /// Represents a meta property that will be output in information requests.
    /// </summary>
    public class MetaPropertyAttribute : Attribute
    {
        /// <summary>
        /// The name of the property.
        /// </summary>
        public string Name;

        /// <summary>
        /// The name of the property to output.
        /// </summary>
        public string Display;

        /// <summary>
        /// The position this property should be in.
        /// </summary>
        public int Position;

        /// <summary>
        /// Whether this property should be displayed inline.
        /// </summary>
        public bool Inline;

        /// <summary>
        /// Force this property to be on the next page.
        /// </summary>
        public bool ForceNextPage;

        /// <summary>
        /// How many members of a list property can be displayed per page.
        /// </summary>
        public int PerPage;

        /// <summary>
        /// Whether this property should be displayed as code.
        /// </summary>
        public bool Code;

        /// <summary>
        /// If this property is displayed as code or has a code block, the extension that should be used for it.
        /// </summary>
        public string CodeExtension = "yml";

        /// <summary>
        /// Holds the <see cref="System.Reflection.PropertyInfo"/> of the property this attribute is defined on.
        /// </summary>
        public PropertyInfo PropertyInfo;
    }
}
