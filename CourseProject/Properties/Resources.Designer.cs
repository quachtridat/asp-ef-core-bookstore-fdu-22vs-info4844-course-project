﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CourseProject.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("CourseProject.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [
        ///  {
        ///    &quot;author&quot;: &quot;Chinua Achebe&quot;,
        ///    &quot;country&quot;: &quot;Nigeria&quot;,
        ///    &quot;imageFileName&quot;: &quot;things-fall-apart.jpg&quot;,
        ///    &quot;language&quot;: &quot;English&quot;,
        ///    &quot;link&quot;: &quot;https://en.wikipedia.org/wiki/Things_Fall_Apart&quot;,
        ///    &quot;pages&quot;: 209,
        ///    &quot;title&quot;: &quot;Things Fall Apart&quot;,
        ///    &quot;year&quot;: 1958,
        ///    &quot;descriptions&quot;: [
        ///      &quot;\u201cA true classic of world literature . . . A masterpiece that has inspired generations of writers in Nigeria, across Africa, and around the world.\u201d \u2014Barack Obama \u201cAfrican literature is [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SeedDataRawJson {
            get {
                return ResourceManager.GetString("SeedDataRawJson", resourceCulture);
            }
        }
    }
}
