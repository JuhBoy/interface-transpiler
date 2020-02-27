using CSharpTranslator.src.Accessors;
using CSharpTranslator.src.SyntaxHelpers;
using System;

namespace EasyTranspiler.src.Accessors
{
    /// <summary>
    /// The strategy doesn't implement all Inclusion Type
    /// Only PropertiesAndFields are possible
    /// TODO: Implement All / Functions
    /// TODO: Implement Comments
    /// </summary>
    interface INodeStrategy
    {
        Type Type { get; }
        string AttributeNameConstraint { get; set; }
        InclusionStrategy InclusionStrategy { get; set; }

        /// <summary>
        /// Produce a CSharpNode with all children from assembly type.
        /// </summary>
        /// <returns>A node CSharpNode</returns>
        CSharpNode ProduceNode();
    }
}
