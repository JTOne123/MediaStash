﻿using Gtk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitcode.MediaStash.Desktop.Views
{
    public class DemoWindow : BaseWindow
    {
        //http://www.mono-project.com/docs/gui/gtksharp/widgets/widget-layout-and-packing/
        public DemoWindow() : base("Demo Window")
        {
            Button button;
            VBox box1;
            HBox box2;
            HSeparator separator;
            Misc misc;
            Box quitbox;
            int which;
            Gtk.Label label;
            Application.Init();

            BorderWidth = 10;

            box1 = new VBox(false, 0);

            switch (1)
            {
                case 1:
                    label = new Gtk.Label("gtk_hbox_new (FALSE, 0);");

                    box2 = new HBox(false, 0);

                    label.SetAlignment(0, 0);
                    box1.PackStart(label, false, false, 0);

                    label.Show();

                    box2 = make_box(false, 0, false, false, 0);

                    box1.PackStart(box2, false, false, 0);
                    box2.Show();

                    box2 = make_box(false, 0, true, false, 0);
                    box1.PackStart(box2, false, false, 0);
                    box2.Show();

                    box2 = make_box(false, 0, true, true, 0);
                    box1.PackStart(box2, false, false, 0);
                    box2.Show();

                    separator = new HSeparator();

                    box1.PackStart(separator, false, true, 5);
                    separator.Show();

                    box1 = new VBox(true, 0);
                    label = new Gtk.Label("gtk_hbox_new (TRUE, 0);");
                    label.SetAlignment(0, 0);

                    box1.PackStart(label, false, false, 0);
                    label.Show();

                    box2 = make_box(true, 0, true, true, 0);

                    box1.PackStart(box2, false, false, 0);
                    box2.Show();

                    box2 = make_box(true, 0, true, true, 0);
                    box1.PackStart(box2, false, false, 0);
                    box2.Show();

                    separator = new HSeparator();

                    box1.PackStart(separator, false, true, 5);
                    separator.Show();

                    break;

                case 2:
                    box2 = new HBox(false, 10);
                    label = new Gtk.Label("gtk_hbox_new (FALSE, 10);");

                    label.SetAlignment(0, 0);
                    box1.PackStart(box2, false, false, 0);
                    box1.Show();

                    box2 = make_box(false, 10, true, true, 0);
                    box1.PackStart(box2, false, false, 0);
                    box2.Show();

                    separator = new HSeparator();

                    box1.PackStart(separator, false, true, 5);
                    separator.Show();

                    box2 = new HBox(false, 0);

                    label = new Gtk.Label("gtk_hbox_new (FALSE, 0);");
                    label.SetAlignment(0, 0);
                    box1.PackStart(label, false, false, 0);
                    label.Show();

                    box2 = make_box(false, 0, true, false, 10);
                    box1.PackStart(box2, false, false, 0);
                    box2.Show();

                    box2 = make_box(false, 0, true, true, 10);
                    box1.PackStart(box2, false, false, 0);
                    box2.Show();

                    separator = new HSeparator();
                    box1.PackStart(separator, false, true, 5);
                    separator.Show();
                    break;

                case 3:
                    box2 = make_box(false, 0, false, false, 0);
                    label = new Label("end");
                    box2.PackEnd(label, false, false, 0);
                    label.Show();

                    box1.PackStart(box2, false, false, 0);
                    box2.Show();

                    separator = new HSeparator();
                    separator.SetSizeRequest(400, 5);
                    box1.PackStart(separator, false, true, 5);
                    separator.Show();
                    break;
            }
            quitbox = new HBox(false, 0);

            button = new Button("Quit");

            quitbox.PackStart(button, true, false, 0);
            box1.PackStart(quitbox, false, false, 0);

            Add(box1);
            button.Show();
            quitbox.Show();

            box1.Show();
        }

        public int x = 50;
        public int y = 50;

        static Gtk.HBox make_box(bool homogeneous, int spacing, bool expand, bool fill, uint padding)
        {
            HBox box;
            Button button;
            string padstr;

            box = new HBox(homogeneous, spacing);
            button = new Button("gtk_box_pack");
            box.PackStart(button, expand, fill, padding);

            button.Show();

            button = new Button("(box,");

            box.PackStart(button, expand, fill, padding);
            button.Show();

            button = new Button("button");
            box.PackStart(button, expand, fill, padding);
            button.Show();

            if (expand == true)
                button = new Button("TRUE");
            else
                button = new Button("FALSE");
            box.PackStart(button, expand, fill, padding);
            button.Show();

            button = new Button(fill ? "TRUE," : "FALSE,");

            box.PackStart(button, expand, fill, padding);
            button.Show();

            padstr = padding.ToString() + ");";

            button = new Button(padstr);
            box.PackStart(button, expand, fill, padding);
            button.Show();
            return box;
        }
    }
}
