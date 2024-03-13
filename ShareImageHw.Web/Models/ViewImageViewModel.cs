using ShareImageHw.Data;

namespace ShareImageHw.Web.Models
{
    public class ViewImageViewModel
    {
        public Image Image { get; set; }
        public bool Authenticated { get; set; }
        public bool FalseAttemt { get; set; }
    }
}
