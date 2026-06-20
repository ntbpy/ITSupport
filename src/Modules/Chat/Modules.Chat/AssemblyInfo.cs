using System.Runtime.CompilerServices;
using MIT.Framework.Web.Modules;

[assembly: FshModule(typeof(MIT.Modules.Chat.ChatModule), 800)]
[assembly: InternalsVisibleTo("Chat.Tests")]
[assembly: InternalsVisibleTo("Integration.Tests")]
