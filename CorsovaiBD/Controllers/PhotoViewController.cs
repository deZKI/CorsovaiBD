using System;
using System.IO;
using AppKit;
using Foundation;


namespace CorsovaiBD
{
    public partial class PhotoViewController : NSViewController
    {
        private byte[] photoData;

        public PhotoViewController(IntPtr handle) : base(handle)
        {
        }

        public void loadPhoto(byte[] photoData)
        {
            
            var imageView = new NSImageView
            {
                Image = NSImage.FromStream(new MemoryStream(photoData)),
                Frame = View.Bounds,
                AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable,
                ImageScaling = NSImageScale.ProportionallyUpOrDown
            };

            View.AddSubview(imageView);
        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //Set up the window content view

            // Create an NSImageView to display the photo
          
        }
    }
}