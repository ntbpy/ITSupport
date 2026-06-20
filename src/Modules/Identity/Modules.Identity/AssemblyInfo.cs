using MIT.Framework.Web.Modules;
using System.Runtime.CompilerServices;

[assembly: FshModule(typeof(MIT.Modules.Identity.IdentityModule), 100)]
[assembly: InternalsVisibleTo("Identity.Tests")]