using System;
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
            this.photoData = photoData;

            var imageView = new NSImageView
            {
                Image = NSImage.FromStream(NSData.FromArray(photoData).AsStream()),
            };

            // Set the image view frame and properties
            imageView.Frame = View.Bounds;
            imageView.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;
            imageView.ImageScaling = NSImageScale.ProportionallyUpOrDown;

            // Add the NSImageView to the content view
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