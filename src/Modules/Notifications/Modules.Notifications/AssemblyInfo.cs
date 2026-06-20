using System.Runtime.CompilerServices;
using MIT.Framework.Web.Modules;

[assembly: FshModule(typeof(MIT.Modules.Notifications.NotificationsModule), 750)]
[assembly: InternalsVisibleTo("Notifications.Tests")]
[assembly: InternalsVisibleTo("Integration.Tests")]
