namespace FashionPass.Tests.Utilities;

public static class Selectors
{
    public static class Header
    {
        public const string Container = "#header_container_id";
        public const string SearchInput = "header input[type='search'], header input[placeholder*='Search' i]";
        public const string LoginLink = "header a[href*='login'], header a[href*='sign-in']";
        public const string BagLink = "header a[href*='bag'], header a[href*='cart']";
    }

    public static class Home
    {
        public const string HeroSection = "#home-video";
        public const string MainContainer = ".base-layout_mainContainer__W19Qj";
        public const string JoinTheClubButton = "a[href*='signup'] .home-banner_try_now_btn__Q_ZMT, .home-banner_try_now_btn__Q_ZMT";
        public const string TryNowButton = "text=TRY NOW";
        public const string TrendingSearchTerm = "White Dress";
    }

    public static class Nav
    {
        public const string BrowseLink = "a[href='/clothing']";
        public const string OccasionsLink = "a[href='/occasions']";
        public const string BrandsLink = "a[href='/brands']";
        public const string HowItWorksLink = "a[href='/how-it-works']";
        public const string GiftCardsLink = "a[href='/gift']";
    }

    public static class Login
    {
        public const string EmailInput = "input[type='email'], input[name='email']";
        public const string PasswordInput = "input[type='password'], input[name='password']";
        public const string SubmitButton = "button[type='submit']";
        public const string ErrorMessage = ".error, [class*='error' i]";
    }

    public static class SignUp
    {
        public const string EmailInput = "input[type='email'], input[name='email']";
        public const string PasswordInput = "input[type='password'], input[name='password']";
        public const string FirstNameInput = "input[name='firstName'], input[name='first_name']";
        public const string LastNameInput = "input[name='lastName'], input[name='last_name']";
        public const string PhoneInput = "input[type='tel'], input[name='phone']";
        public const string SubmitButton = "button[type='submit']";
    }

    public static class Plans
    {
        public const string PlanCard = "[class*='plan' i]";
        public const string SelectButton = "text=Select";
    }

    public static class ProductListing
    {
        public const string ProductLink = "a[href*='/products/'], a[href*='/product/'], a[href*='/designs/']";
        public const string CategoryTitle = "h1";
    }

    public static class ProductDetail
    {
        public const string ProductName = "h1";
        public const string AddToBagButton = "text=Add to Bag";
        public const string SizeSelector = "[class*='size' i]";
    }

    public static class Bag
    {
        public const string ItemCount = "[class*='item-count' i], [class*='bag-count' i]";
        public const string CheckoutButton = "text=Checkout";
    }

    public static class Footer
    {
        public const string Container = ".footer_container";
        public const string AboutUsLink = "a[href='/about-us']";
        public const string FaqLink = "a[href='/faq']";
    }
}