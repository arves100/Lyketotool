using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Lyketo.Formats;
using Lyketo.JSON;

namespace Lyketo
{
    /// <summary>
    /// Main program execution.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// Application entrypoint.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
#if false
            if (args.Contains("--nogui"))
            {
                NoGuiMode(args);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new YAMTForm());

#endif

            // ----------------------------------------------------
            // BEGIN TEST MODE
            // ----------------------------------------------------

            JSONParser json = new JSONParser();

            if (!json.Parse(@"yatpproto_def.json"))
                throw new Exception("JSON error");

            uint[] keys =
            {
                173217,
                72619434,
                408587239,
                27973291
            };

            //XMLFormat src = new XMLFormat();
            LZOFormat src = new LZOFormat(keys, false);
            XMLFormat dst = new XMLFormat();

            if (!src.Initialize(@"item_proto", false))
                throw new Exception("SRC init error");

            if (!dst.Initialize(@"yamttest.xml", true))
                throw new Exception("DST init error");

            ProtoFactory.ProcessItemProto(json, src, dst);

            if (!src.Finalize())
                throw new Exception("SRC end error");

            if (!dst.Finalize())
                throw new Exception("DST end error");
        }
    }
}
