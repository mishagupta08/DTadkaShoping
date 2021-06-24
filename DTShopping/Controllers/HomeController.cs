using System.Web.Mvc;
using DTShopping.Repository;
using DTShopping.Properties;
using System;
using System.Collections.Generic;
using DTShopping.Models;
using System.Threading.Tasks;
using System.Linq;
using PagedList;
using System.Threading;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using InstamojoAPI;
using System.IO;
using System.Net;
using System.Data;

namespace DTShopping.Controllers
{
    public class HomeController : Controller
    {
        APIRepository objRepository = new APIRepository();
        General clsgen = new General();
        Dashboard model = new Dashboard();
        private const int SUNVISCOMPANYID = 29;
        string Theme = System.Configuration.ConfigurationManager.AppSettings["Theme"] == null ? string.Empty : System.Configuration.ConfigurationManager.AppSettings["Theme"].ToString();
        string companyId = System.Configuration.ConfigurationManager.AppSettings["CompanyId"];
        // string Insta_client_id = "tmLkZZ0zV41nJwhayBGBOI4m4I7bH55qpUBdEXGS",
        //Insta_client_secret = "IDejdccGqKaFlGav9bntKULvMZ0g7twVFolC9gdrh9peMS0megSFr7iDpWwWIDgFUc3W5SlX99fKnhxsoy6ipdAv9JeQwebmOU6VRvOEQnNMWwZnWglYmDGrfgKRheXs",
        // Insta_Endpoint = InstamojoConstants.INSTAMOJO_API_ENDPOINT,
        //Insta_Auth_Endpoint = InstamojoConstants.INSTAMOJO_AUTH_ENDPOINT;
        string Insta_client_id = "MgmTUc8vZDPbv1gaNrcyoHJldS8Slnrol91ljIVs",
       Insta_client_secret = "vb5CVqPjz0PsUIG3iWVWOreVlElkaQ9EPBeiPaWVrAVtnkyz0JB21oI5cpHyZlxY3gm6AwV4RheXUBduwhukDV1uPwxY4yJxG83v0hXVgosODRWFrNqZCcHSC0P5Xkga",
       Insta_Endpoint = "https://www.instamojo.com/v2/",
       Insta_Auth_Endpoint = "https://www.instamojo.com/oauth2/token/";

        public async Task<ActionResult> Index()
        {
            Dashboard objDashboardDetails = new Dashboard();

            try
            {
                var res = await this.SetCompanyDetailInSession();
                if (res != null)
                {
                    return res;
                }
                //var dt = new List<company>();
                //dt.Add(new company
                //{
                //    id = Convert.ToInt32(companyId)
                //});

                //var companyDetail = await objRepository.GetCompanyById(dt);
                //if (companyDetail == null || companyDetail.default_flag == 0)
                //{
                //    return View("Error");
                //}
                //else
                {
                    //Session["Company"] = companyDetail;
                    objDashboardDetails.Banners = new List<Banners>();
                    objDashboardDetails.Banners = await objRepository.GetBannerImageList(companyId);

                    objDashboardDetails.FontpageSections = new ShoppingPortalFrontPageProdList();
                    objDashboardDetails.FontpageSections = await objRepository.GetShoppingPortalFrontPageProdList(companyId);

                    Session["LatestProduct"] = objDashboardDetails.FontpageSections.SpeacialSegment;
                    Session["Brands"] = objDashboardDetails.FontpageSections.brandlist;

                }
            }
            catch (Exception ex)
            {

            }

            if (Theme == Resources.Orange)
            {
                return View("IndexOrange", objDashboardDetails);
            }
            else
            {
                return View("Index", objDashboardDetails);
            }
        }

        public async Task<ActionResult> TermsAndConditions()
        {
            var res = await this.SetCompanyDetailInSession();
            if (res != null)
            {
                return res;
            }

            return View();
        }

        public async Task<ActionResult> PrivacyPolicy()
        {
            var res = await this.SetCompanyDetailInSession();
            if (res != null)
            {
                return res;
            }

            return View();
        }

        public async Task<ActionResult> About()
        {
            ViewBag.Message = "Your application description page.";
            var res = await this.SetCompanyDetailInSession();
            var profile = new CompanyProfile { CompanyId = Convert.ToInt32(companyId) };
            //get companyProfileDetail
            var result = await objRepository.ManageCompanyProfile(profile, "ByCompanyId");
            if (!(result == null || result.Status == false))
            {
                this.model = new Dashboard();
                this.model.CompanyProfileDetail = JsonConvert.DeserializeObject<CompanyProfile>(result.ResponseValue);
            }

            return View(this.model);
        }

        public async Task<ActionResult> Contact()
        {
            var dt = new List<company>();
            string companyId = System.Configuration.ConfigurationManager.AppSettings["CompanyId"];
            dt.Add(new company
            {
                id = Convert.ToInt32(companyId)
            });

            var companyDetail = await objRepository.GetCompanyById(dt);
            if (companyDetail == null || companyDetail.default_flag == 0)
            {
                return View("Error");
            }
            this.model = new Dashboard();
            this.model.CompanyDetail = companyDetail;
            return View(this.model);
        }

        public async Task<ActionResult> GetProductDetail(int prodId)
        {
            if (CheckLoginUserStatus())
            {
                var dashboard = new Dashboard();
                this.objRepository = new APIRepository();
                try
                {
                    var prodList = new List<Product>();

                    prodList.Add(new Product { id = prodId });
                    if (Theme == Resources.Orange)
                    {
                        var container = await objRepository.GetProductDetailByIdForOrangeTheme(prodList);
                        if (container != null)
                        {
                            dashboard.ProductDetail = container.ProductDetail;
                            dashboard.RelatedProductList = container.RelatedProductList;
                            dashboard.SameBrandProductList = container.SameBrandProductList;
                        }
                    }
                    else
                    {
                        dashboard.ProductDetail = await objRepository.GetProductDetailById(prodList);
                    }

                    if (dashboard.ProductDetail != null)
                    {
                        if (!string.IsNullOrEmpty(dashboard.ProductDetail.description_detail))
                        {
                            dashboard.ProductDetail.description_detail = dashboard.ProductDetail.description_detail.Replace("\r\n\r\n", "");
                        }
                        if (!string.IsNullOrEmpty(dashboard.ProductDetail.product_size))
                        {
                            dashboard.ProductDetail.sizeList = dashboard.ProductDetail.product_size.Split(',').ToList();
                        }
                        if (!string.IsNullOrEmpty(dashboard.ProductDetail.Color))
                        {
                            dashboard.ProductDetail.colorList = dashboard.ProductDetail.Color.Split(',').ToList();
                        }

                        var prodImageList = new List<product_images>();

                        prodImageList.Add(new product_images { product_id = prodId });
                        dashboard.ProductDetail.prodImageList = await objRepository.GetProductImagesList(prodImageList);
                    }
                }
                catch (Exception ex)
                {

                }

                if (Theme == Resources.Orange)
                {
                    return View("productDetailPageOrange", dashboard);
                }
                else
                {
                    return View("productDetailPage", dashboard);
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public async Task<ActionResult> ProductList(string cat, string BrandId, string root, int? page, string SortBy, string Order, string FilterFromPoint, string FilterToPoint, string searchString, string pointsFilterList)
        {
            this.model = new Dashboard();
            Filters c = new Filters();
            if (!string.IsNullOrEmpty(cat))
            {
                c.CategoryId = Convert.ToInt16(cat);
            }

            if (!string.IsNullOrEmpty(BrandId))
            {
                c.BrandId = Convert.ToInt16(BrandId);
            }

            c.pageNo = page ?? 1;
            c.NoOfRecord = 10;
            c.SelectedFilterName = "Title";
            c.FilterValue = searchString;
            if (!string.IsNullOrEmpty(SortBy))
            {
                if (SortBy.ToLower() == "price")
                {
                    c.SortByPrice = true;
                    if (Order.ToLower() == "asc")
                    {
                        c.IsPriceLowToHigh = true;
                    }
                    else
                    {
                        c.IsPriceLowToHigh = false;
                    }
                }
                else if (SortBy.ToLower() == "points")
                {
                    c.SortByPoints = true;
                    if (Order.ToLower() == "asc")
                    {
                        c.IsPointLowToHigh = true;
                    }
                    else
                    {
                        c.IsPointLowToHigh = false;
                    }
                }
            }
            if (!string.IsNullOrEmpty(FilterFromPoint) && !string.IsNullOrEmpty(FilterToPoint))
            {
                c.FilterFromPoint = Convert.ToInt32(FilterFromPoint);
                c.FilterToPoint = Convert.ToInt32(FilterToPoint);
            }

            if (!string.IsNullOrEmpty(pointsFilterList))
            {
                var index = pointsFilterList.LastIndexOf(',');
                pointsFilterList = pointsFilterList.Remove(index);
                var pintList = pointsFilterList.Split(',').ToList().ConvertAll(int.Parse);
                if (pintList.Count() >= 2)
                {
                    c.FilterFromPoint = Convert.ToInt32(pintList[0]);
                    c.FilterToPoint = Convert.ToInt32(pintList[pintList.Count() - 1]);
                }
            }

            var result = await objRepository.GetCategoryProducts(c);
            List<Product> listProducts = new List<Product>();
            double totalcount = 0;
            if (result.Status == true && result.ResponseValue != null)
            {
                totalcount = result.TotalRecords;
                listProducts = JsonConvert.DeserializeObject<List<Product>>(result.ResponseValue);
            }

            string catName = string.Empty;
            if (!string.IsNullOrEmpty(cat))
            {
                var catDetail = await objRepository.GetCategoryDetail(c);
                catName = catDetail.title;
            }
            //if (!string.IsNullOrEmpty(BrandId))
            //{
            //    var brandDetail = await objRepository.GetBrandDetail(c);
            //    brandDetail = catDetail.title;
            //}

            PagewiseProducts finalprodlist = new PagewiseProducts();
            finalprodlist.ProductList = listProducts;

            var list = new List<int>();
            for (var i = 1; i <= totalcount; i++)
            {
                list.Add(i);
            }

            finalprodlist.pagerCount = list.ToPagedList(Convert.ToInt32(c.pageNo), 10);


            ViewBag.category = cat;
            ViewBag.CategoryName = catName;
            ViewBag.Page = page;
            ViewBag.ParentId = root;
            ViewBag.brand = BrandId;
            this.model.finalProductList = finalprodlist;
            await SetCompanyDetailInSession();
            this.model.FilterDetail = c;
            if (Theme == Resources.Orange)
            {
                //this.model.FontpageSections = new ShoppingPortalFrontPageProdList();
                //this.model.FontpageSections = await objRepository.GetShoppingPortalFrontPageProdList(companyId);

                //if (finalprodlist != null && finalprodlist.ProductList != null && finalprodlist.ProductList.Count > 0)
                //{
                //    var pId = finalprodlist.ProductList.FirstOrDefault().id;
                //    var prodList = new List<Product>();
                //    prodList.Add(new Product { id = pId });


                //    var container = await objRepository.GetProductDetailByIdForOrangeTheme(prodList);
                //    if (container != null)
                //    {
                //        this.model.RelatedProductList = container.RelatedProductList;
                //    }

                //}
                this.model.IdList = searchString;
                return View("ProductListOrange", this.model);
            }
            else
            {
                return View(this.model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetCategories()
        {
            List<Category> list = new List<Category>();
            try
            {
                //if (Session["MenuList"] != null)
                //{

                //}
                //else
                //{
                var MenuItems = await objRepository.GetMenuList();
                if (MenuItems != null)
                {
                    MenuItems = MenuItems.Where(r => r.status == true).OrderBy(r => r.ordar).ToList();
                    list = getNestedChildren(MenuItems.Where(r => r.status == true && r.parent_id == 1 && r.parent_id != r.id).ToList(), MenuItems.Where(r => r.status == true).ToList());
                    Session["MenuList"] = list;
                }
                //}                                                      
            }
            catch (Exception ex)
            {

            }

            if (Theme == Resources.Orange)
            {
                return PartialView("CategoryOrange", list);
            }
            else
            {
                return PartialView("Category", list);
            }
        }

        public ActionResult getCatHeirarchy(string Cat, string subCat, string brandId, string minPointValue, string maxPointValue)
        {
            SideBar objsidebar = new SideBar();
            var CategoryList = new List<Category>();
            int categor = 0;
            if (!string.IsNullOrEmpty(Cat))
            {
                categor = Convert.ToInt16(Cat);
            }
            if (Session["MenuList"] != null && categor != 0)
            {
                var MenuItems = Session["MenuList"] as List<Category>;
                CategoryList = getNestedChildren(MenuItems.Where(r => r.status == true && r.id == categor).ToList(), MenuItems);
            }
            ViewBag.ParentId = Cat;
            ViewBag.category = subCat;
            ViewBag.brand = brandId;
            ViewBag.Page = 1;
            objsidebar.categoryList = CategoryList;
            objsidebar.filterDetail = new Filters();

            if (string.IsNullOrEmpty(minPointValue))
            {
                objsidebar.filterDetail.FilterFromPoint = null;
            }
            else
            {
                objsidebar.filterDetail.FilterFromPoint = Convert.ToInt32(minPointValue);
            }

            if (string.IsNullOrEmpty(maxPointValue))
            {
                objsidebar.filterDetail.FilterToPoint = null;
            }
            else
            {
                objsidebar.filterDetail.FilterToPoint = Convert.ToInt32(maxPointValue);
            }

            if (Session["LatestProduct"] != null)
            {
                var product = Session["LatestProduct"] as List<Product>;
                objsidebar.latestProduct = product.FirstOrDefault();
            }

            if (Theme == Resources.Orange)
            {
                return PartialView("_filterSideBarOrange", objsidebar);
            }
            else
            {
                return PartialView("_filterSideBar", objsidebar);
            }
        }

        public List<Category> getNestedChildren(List<Category> ParentList, List<Category> MenuList)
        {
            var orderedList = new List<Category>();
            if (ParentList.Count > 0)
            {
                foreach (var parent in ParentList)
                {
                    var submenu = MenuList.Where(r => r.parent_id == parent.id).ToList();
                    if (submenu.Count > 0 && !(parent.id == parent.parent_id))
                    {
                        parent.Childern = new List<Category>();
                        parent.Childern = (getNestedChildren(submenu, MenuList));
                    }
                    orderedList.Add(parent);
                }

            }
            return orderedList;
        }

        public ActionResult Claims()
        {
            return View();
        }

        public async Task<ActionResult> SearchProductList(int? page, string SortBy, string Order, string searchString)
        {
            this.model = new Dashboard();
            Filters c = new Filters();

            c.pageNo = page;
            c.NoOfRecord = 10;
            c.SelectedFilterName = "Title";
            c.FilterValue = searchString;
            if (!string.IsNullOrEmpty(SortBy))
            {
                if (SortBy.ToLower() == "price")
                {
                    c.SortByPrice = true;
                    if (Order.ToLower() == "asc")
                    {
                        c.IsPriceLowToHigh = true;
                    }
                    else
                    {
                        c.IsPriceLowToHigh = false;
                    }
                }
                else if (SortBy.ToLower() == "points")
                {
                    c.SortByPoints = true;
                    if (Order.ToLower() == "asc")
                    {
                        c.IsPointLowToHigh = true;
                    }
                    else
                    {
                        c.IsPointLowToHigh = false;
                    }
                }
            }

            var result = await objRepository.GetCategoryProducts(c);
            List<Product> listProducts = new List<Product>();
            double totalcount = 0;
            if (result.Status == true && result.ResponseValue != null)
            {
                totalcount = result.TotalRecords;
                listProducts = JsonConvert.DeserializeObject<List<Product>>(result.ResponseValue);
            }

            PagewiseProducts finalprodlist = new PagewiseProducts();
            finalprodlist.SearchString = searchString;
            finalprodlist.ProductList = listProducts;

            var list = new List<int>();
            for (var i = 1; i <= totalcount; i++)
            {
                list.Add(i);
            }
            finalprodlist.pagerCount = list.ToPagedList(Convert.ToInt32(page), 10);
            ViewBag.Page = page;
            this.model.finalProductList = finalprodlist;
            if (Theme == Resources.Orange)
            {
                this.model.FontpageSections = new ShoppingPortalFrontPageProdList();
                this.model.FontpageSections = await objRepository.GetShoppingPortalFrontPageProdList(companyId);

                if (finalprodlist != null && finalprodlist.ProductList != null && finalprodlist.ProductList.Count > 0)
                {
                    var pId = finalprodlist.ProductList.FirstOrDefault().id;
                    var prodList = new List<Product>();
                    prodList.Add(new Product { id = pId });


                    var container = await objRepository.GetProductDetailByIdForOrangeTheme(prodList);
                    if (container != null)
                    {
                        this.model.RelatedProductList = container.RelatedProductList;
                    }

                }

                return View("SearchProductListOrange", this.model);
            }
            else
            {
                return View(this.model);
            }
        }

        public async Task<ActionResult> GetAllDealProducts(string Deal, int? page, string SortBy, string Order, int? catId)
        {
            PagewiseProducts productlist = new PagewiseProducts();
            try
            {
                Filters c = new Filters();
                string companyId = System.Configuration.ConfigurationManager.AppSettings["CompanyId"];

                c.CompanyId = Convert.ToInt16(companyId);
                c.pageNo = page;
                c.NoOfRecord = 10;
                productlist.SearchString = Deal;
                productlist.sortby = SortBy;
                productlist.order = Order;
                productlist.CatId = catId??0;
                c.CategoryId = catId;

                if (!string.IsNullOrEmpty(SortBy))
                {
                    if (SortBy.ToLower() == "price")
                    {
                        c.SortByPrice = true;
                        if (Order.ToLower() == "asc")
                        {
                            c.IsPriceLowToHigh = true;
                        }
                        else
                        {
                            c.IsPriceLowToHigh = false;
                        }
                    }
                    else if (SortBy.ToLower() == "points")
                    {
                        c.SortByPoints = true;
                        if (Order.ToLower() == "asc")
                        {
                            c.IsPointLowToHigh = true;
                        }
                        else
                        {
                            c.IsPointLowToHigh = false;
                        }
                    }
                }

                var result = await objRepository.GetDealProductsFullList(c, Deal);
                List<Product> listProducts = new List<Product>();
                double totalcount = 0;
                if (result.Status == true && result.ResponseValue != null)
                {
                    totalcount = result.TotalRecords;
                    listProducts = JsonConvert.DeserializeObject<List<Product>>(result.ResponseValue);
                }

                productlist.ProductList = listProducts;

                var list = new List<int>();
                for (var i = 1; i <= totalcount; i++)
                {
                    list.Add(i);
                }

                productlist.pagerCount = list.ToPagedList(Convert.ToInt32(page), 10);
                ViewBag.Page = page;
            }
            catch (Exception ex)
            {
            }
            return View("DealProducts", productlist);

        }

        public ActionResult MyAccount()
        {
            return View();
        }

        public ActionResult UpdateAccount()
        {
            var userDetail = Session["UserDetail"] as UserDetails;
            return View(userDetail);
        }

        public ActionResult DiscountCoupons()
        {
            return View();
        }

        public async Task<ActionResult> Orders(int? page)
        {
            var dash = new Dashboard();
            if (Session["UserDetail"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var userID = 0;
                var companyID = 0;
                Filters objFilter = new Filters();
                //PagedOrderList UserOrderList = new PagedOrderList();
                if (Session["UserDetail"] != null)
                {
                    userID = (Session["UserDetail"] as UserDetails).id;
                    companyID = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["CompanyId"]);

                    objFilter.CompanyId = companyID;
                    objFilter.VendorId = userID;
                    objFilter.pageNo = page;
                    objFilter.pageName = "VendorOrderProductList";
                    objFilter.SelectedFilterName = "userOrderList";
                    var result = await objRepository.GetUserOrderList(objFilter);
                    double totalcount = 0;
                    if (result.Status == true && result.ResponseValue != null)
                    {
                        totalcount = result.TotalRecords;
                        dash.orderDetailListContainer = JsonConvert.DeserializeObject<List<OrderDetailContainer>>(result.ResponseValue);
                    }

                    var list = new List<int>();
                    for (var i = 1; i <= totalcount; i++)
                    {
                        list.Add(i);
                    }

                    dash.pagerCount = list.ToPagedList(Convert.ToInt32(page ?? 1), 10);

                }
                return View(dash);
            }
        }

        private bool CheckLoginUserStatus()
        {
            if (Session["UserDetail"] == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<ActionResult> Checkout(int deliveryType)
        {
            order objUserOrder = new order();
            try
            {
                if (Session["UserDetail"] == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    var userDetail = Session["UserDetail"] as UserDetails;
                    objUserOrder.user_id = userDetail.id;
                    var result = await objRepository.GetUserOpenOrder(objUserOrder);
                    if (result != null)
                    {
                        objUserOrder = result;
                    }
                    else
                    {
                        objUserOrder.billing_city = userDetail.CityName;
                        objUserOrder.billing_state = userDetail.StateName;
                        objUserOrder.billing_phone = userDetail.phone;
                        objUserOrder.user_id = userDetail.id;
                        objUserOrder.billing_first_name = userDetail.first_name;
                        objUserOrder.billing_last_name = userDetail.last_name;
                    }

                    this.model = new Dashboard();
                    this.model.OrderDetail = objUserOrder;
                    this.model.OrderDetail.delievryType = (deliveryType);
                    //this.model.deliveryTypeList = await objRepository.DeliveryTypeList();
                    await AssignStateCityList(objUserOrder.billing_state);
                }
            }
            catch (Exception ex)
            {

            }

            if (Theme == Resources.Orange)
            {
                var manage = new ManageController();
                var usr = Session["UserDetail"] as UserDetails;
                await manage.SetShoppingCartProductInModel(this.model, usr);
                return View("CheckoutOrange", this.model);
            }
            else
            {
                return View(this.model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> getCartCount()
        {
            UserDetails userDetail = new UserDetails();
            Response userResponse = new Response();
            try
            {
                if (Session["UserDetail"] == null)
                {
                    userResponse.Points = 0;
                    userResponse.CartProductCount = 0;
                }
                else
                {
                    userDetail = Session["UserDetail"] as UserDetails;
                    var result = await objRepository.getCartCount(userDetail);

                    if (result.Status == true)
                    {
                        //userResponse = result;
                        
                        userResponse.CartProductCount = result.CartProductCount;
                        userResponse.totalPoints = Convert.ToString(result.Points) + "" +"(Balance Point 5000 (Monthly Limit 500))";
                        userResponse.Points = result.Points;
                        userResponse.Url = result.Url;
                        userResponse.ResponseValue = result.ResponseValue;
                        userResponse.Status = result.Status;
                        userResponse.TotalRecords = result.TotalRecords;
                         if(result.Points >0 && companyId == "30")
                        {
                            Session["Points"] = result.Points;
                        }
                        else
                        {
                            Session["Points"] = Convert.ToString(result.Points) + "Balance Point 5000 (Monthly Limit 500)";
                        }
                        Session["Points"] = result.Points;
                        Session["CartNumber"] = result.CartProductCount;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return Json(userResponse, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrder(Dashboard obj)
        {
            var orderstatus = string.Empty;
            order objorder = obj.OrderDetail;
            try
            {
                objorder.created = DateTime.Now;
                //objorder.status = 2;
                objorder.company_id = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["CompanyId"]);
                if (objorder.company_id == SUNVISCOMPANYID)
                {
                    UserDetails current_user = (Session["UserDetail"] != null) ? Session["UserDetail"] as UserDetails : null;
                    if (current_user != null)
                    {
                        objorder.billing_first_name = current_user.first_name;
                        objorder.billing_last_name = current_user.last_name;
                    }
                }

                Response response = new Response();
                if (objorder.id == 0)
                {
                    response = await objRepository.CreateOrder(objorder, "Add");
                }
                else
                {
                    response = await objRepository.CreateOrder(objorder, "EditAddress");
                }

                if (response.Status == true)
                {
                    Session["OrderId"] = response.ResponseValue;
                    orderstatus = "Success";
                    //return RedirectToAction("GetCartProductList", "Manage", new { isWithPayment = true });
                }
                else
                {
                    orderstatus = "Fail";
                }
            }
            catch (Exception ex)
            {
                orderstatus = "Fail";
            }

            return
                Json(orderstatus, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateAccount(UserDetails objorder)
        {
            var orderstatus = string.Empty;
            try
            {
                objorder.modified = DateTime.Now;
                var response = await objRepository.UpdateAccount(objorder);
                if (response.Status == true)
                {
                    orderstatus = response.ResponseValue;
                }
                else
                {
                    orderstatus = "Fail";
                }
            }
            catch (Exception ex)
            {
                orderstatus = "Fail";
            }
            return Json(orderstatus, JsonRequestBehavior.AllowGet);
        }

        private async Task AssignStateCityList(string biilingState)
        {

            this.model.States = await this.objRepository.GetStateList();
            if (this.model.States == null)
            {
                this.model.States = new List<R_StateMaster>();
                this.model.States.Add(new R_StateMaster
                {
                    Id = 0,
                    Name = "-Not Available-"
                });
            }

            if (!string.IsNullOrEmpty(biilingState))
            {
                this.model.Cities = await this.objRepository.GetCityListById(biilingState);
            }

            if (this.model.Cities == null)
            {
                this.model.Cities = new List<R_CityMaster>();
                this.model.Cities.Add(new R_CityMaster
                {
                    cityID = 0,
                    cityName = "-Not Available-"
                });
            }
        }

        public async Task<ActionResult> GetCityListByState(string Id)
        {
            var cityList = new List<R_CityMaster>();
            this.objRepository = new APIRepository();
            this.model = new Dashboard();
            if (string.IsNullOrEmpty(Id))
            {
                return null;
            }
            else
            {
                cityList = await this.objRepository.GetCityListById(Id);
            }

            if (cityList == null || cityList.Count == 0)
            {
                cityList = new List<R_CityMaster>();
                if (this.model.Cities == null)
                {
                    this.model.Cities = new List<R_CityMaster>();
                    this.model.Cities.Add(new R_CityMaster
                    {
                        cityID = 0,
                        cityName = "-Not Available-"
                    });
                }
            }
            return Json(cityList);
        }

        public async Task<ActionResult> SetCompanyDetailInSession()
        {
            if (Session["Company"] == null)
            {
                try
                {
                    var dt = new List<company>();
                    dt.Add(new company
                    {
                        id = Convert.ToInt32(companyId)
                    });

                    var companyDetail = await objRepository.GetCompanyById(dt);
                    if (companyDetail == null || companyDetail.default_flag == 0)
                    {
                        return View("Error");
                    }
                    else
                    {
                        Session["Company"] = companyDetail;
                    }
                }
                catch (Exception e)
                {
                    // return null;
                }
            }

            return null;
        }

         public async Task<ActionResult>BuyPackage()
        {
            return View();
        }
         public async Task<ActionResult> ValidatePackage(string KitID)
        {
           // UserDetails user = new UserDetails();
               var  user =  Session["UserDetail"] as UserDetails;
            var username = user.username;
            var Combo = username+"/"+ KitID;
            var encode=Base64Encode(Combo);
            var decode = Base64Decode(encode);
            HttpClient client = new HttpClient();
            var path = "http://gohappy.gohappynetwork.com/CoinResponse.aspx?checkid=" + encode ;
            var response = await client.GetAsync(path);
            var result = await response.Content.ReadAsStringAsync();
            var myList = JsonConvert.DeserializeObject<ResponseBuyPackage>(result);
            
            ApiPinCoderesponse Code = new ApiPinCoderesponse();
            {
                Code.request = encode;
                Code.response = result;
                Code.url = path;
            }
            var statusID = await this.objRepository.SaveAPIRequest(Code);

            return Json(myList);

        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
         public async Task<ActionResult> SaveBuyPackage(string KitId)
        {
            var user = Session["UserDetail"] as UserDetails;
            var username = user.username;
            var Combo = username + "/" + KitId;
            var encode = Base64Encode(Combo);
            var decode = Base64Decode(encode);
            var path = "http://gohappy.gohappynetwork.com/CoinResponse.aspx?token=" + encode;
            ApiPinCoderesponse Code = new ApiPinCoderesponse();
            {
                Code.request = encode;
                Code.response = "";
                Code.url = path;
            }
            var statusID = await this.objRepository.SaveAPIRequest(Code);

            return Redirect("http://gohappy.gohappynetwork.com/CoinResponse.aspx?token=" + encode);
        }
         public async Task<ActionResult> ValidateWalletPackage(string KitId)
        {
            var user = Session["UserDetail"] as UserDetails;
            var username = user.username;
            var Combo = username + "/" + KitId;
            var encode = Base64Encode(Combo);
            var decode = Base64Decode(encode);
            var path = "http://gohappy.gohappynetwork.com/CoinResponse.aspx?wallet=" + encode;
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(path);
            var result = await response.Content.ReadAsStringAsync();
            var myList = JsonConvert.DeserializeObject<ResponseBuyPackage>(result);

            ApiPinCoderesponse Code = new ApiPinCoderesponse();
            {
                Code.request = encode;
                Code.response = result;
                Code.url = path;
            }
            var statusID = await this.objRepository.SaveAPIRequest(Code);
            return Json(myList);
        }

         public async Task<ActionResult> BtnPGProceed(string Amount)
        {
            string Url = "";
            //Amount = "10";
             if (companyId=="30")
            {
                var user = Session["UserDetail"] as UserDetails;
                string agentid = "";
                agentid = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                PaymentRequest pay = new PaymentRequest()
                {
                    UserId = Convert.ToString(user.id),
                    Amount = Convert.ToDecimal(Amount),
                    OrderID = agentid,
                    TID = "",
                    TDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    Status = "",
                    UserName = Convert.ToString(user.username)

                };
                //StringBuilder sb = new StringBuilder();
                //sb.AppendLine("<PaymentRequest>");
                //sb.AppendLine(("<UserId>"
                //                + (Convert.ToString(user.id) + "</UserId>")));
                //sb.AppendLine(("<UserName>"
                //              + (Convert.ToString(user.username) + "</UserName>")));
                //sb.AppendLine(("<Amount>"
                //               + Amount + "</Amount>"));
                //sb.AppendLine(("<OrderID>"
                //               + agentid + "</OrderID>"));
                //sb.AppendLine(("<TID></TID>"));
                //sb.AppendLine(("<TDate>"
                //             + DateTime.Now.ToString("yyyy-MM-dd") + "</TDate>"));
                //sb.AppendLine(("<Status></Status>"));
                //sb.AppendLine("</PaymentRequest>");

                DataSet ds = null;
                ds = new DataSet();
                var statusID = await this.objRepository.SavePaymentRequest(pay);
                  Url=instamojopg(Amount, agentid);

            }
            return Json(Url);
           // return Redirect(Url);
        }

        private string instamojopg(string Amount, string agentid)
        {
            var user = Session["UserDetail"] as UserDetails;

            string paymentUrl = "";
            //var s = await this.objRepository.SavePaymentRequest(Convert.ToString(sb));

            Instamojo objClass = InstamojoImplementation.getApi(Insta_client_id, Insta_client_secret, Insta_Endpoint, Insta_Auth_Endpoint);
            //Create Payment Order

          PaymentOrder objPaymentRequest = new PaymentOrder();
            //Required POST parameters
            objPaymentRequest.name = user.first_name;
            objPaymentRequest.email = Convert.ToString(user.email);
            objPaymentRequest.phone = Convert.ToString(user.phone);
            objPaymentRequest.description = "Test description";
            objPaymentRequest.amount = Convert.ToDouble(Amount);
            objPaymentRequest.currency = "USD";
            string randomName = Path.GetRandomFileName();
            randomName = randomName.Replace(".", string.Empty);
            objPaymentRequest.transaction_id = agentid;
            //objPaymentRequest.redirect_url = "https://swaggerhub.com/api/saich/pay-with-instamojo/1.0.0";
            //objPaymentRequest.redirect_url = "http://localhost:63692/Home/PaymentPgResponse";
            objPaymentRequest.redirect_url = "http://gohappycart.com/Home/PaymentPgResponse";
            objPaymentRequest.webhook_url = "http://gohappycart.com/Home/PaymentPgResponse";
            try
            {
                if (objPaymentRequest.validate())
                {
                    if (objPaymentRequest.nameInvalid)
                    {
                       // MessageBox.Show("Name is not valid");
                    }
                    else if (objPaymentRequest.phoneInvalid)
                    {
                        //MessageBox.Show("Name is not valid");
                    }
                    else if (objPaymentRequest.redirectUrlInvalid)
                    {
                        //MessageBox.Show("Name is not valid");
                    }
                    else
                    {


                    }
                }
                else
                {

                    try
                    {
                        CreatePaymentOrderResponse objPaymentResponse = objClass.createNewPaymentRequest(objPaymentRequest);
                        string orderId = objPaymentResponse.order.id;
                         paymentUrl = objPaymentResponse.payment_options.payment_url;
                        //MessageBox.Show("Payment URL = " + objPaymentResponse.payment_options.payment_url);
                    }
                    catch (ArgumentNullException ex)
                    {
                       // MessageBox.Show(ex.Message);
                    }
                    catch (WebException ex)
                    {
                        //MessageBox.Show(ex.Message);
                    }
                    catch (IOException ex)
                    {
                        //MessageBox.Show(ex.Message);
                    }
                    catch (InvalidPaymentOrderException ex)
                    {
                        if (!ex.IsWebhookValid())
                        {
                            //MessageBox.Show("Webhook is invalid");
                        }

                        if (!ex.IsCurrencyValid())
                        {
                            //MessageBox.Show("Currency is Invalid");
                        }

                        if (!ex.IsTransactionIDValid())
                        {
                            //MessageBox.Show("Transaction ID is Inavlid");
                        }
                    }
                    catch (ConnectionException ex)
                    {
                       // MessageBox.Show(ex.Message);
                    }
                    catch (BaseException ex)
                    {
                       // MessageBox.Show(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show("Error:" + ex.Message);
                    }
                }
                //MessageBox.Show("Order Id = " + objPaymentResponse.order.id);
            }
            catch (ArgumentNullException ex)
            {
               // MessageBox.Show(ex.Message);
            }
            return paymentUrl;

        }

         public async Task<ActionResult> PaymentPgResponse()
        {
            //IPayement paymentDb = null;
            General clsgen = new General();
            Boolean verifyHash;
            string salt = "";
            //MerchantAPI.ChecksumCalculator objCalc = new MerchantAPI.ChecksumCalculator();
            StringBuilder sb = new StringBuilder();
            try
            {

                string responseCode = string.Empty;
                string uniqueRefID = string.Empty;
                string tAmount = "0";
                string tDate = string.Empty;
                string status = string.Empty;
                string orderId = string.Empty;
                string rs = string.Empty;
                string rrnNo = string.Empty;
                string rsv = string.Empty;
                string mandatoryField = string.Empty;
                string rMessage = string.Empty;
                string res = string.Empty;
                string id = string.Empty;
                //response = HttpContext.Current.Request.ToString();
                //uniqueRefID = HttpContext.Current.Request["payment_id"].ToString();
                //status = HttpContext.Current.Request["payment_status"].ToString();
                //orderId = HttpContext.Current.Request["transaction_id"].ToString();
                //response = HttpContext.Current.Request.QueryString.ToString();
                res = HttpContext.Request.ToString();
                uniqueRefID =Request.Params["payment_id"].ToString();
                status=Request.Params["payment_status"].ToString();
                orderId=Request.Params["transaction_id"].ToString();
                tAmount = Request.Params["amount"];
                id = Request.Params["id"];
                var user = Session["UserDetail"] as UserDetails;
                InstamojoAPI.Instamojo objClass = InstamojoAPI.InstamojoImplementation.getApi(Insta_client_id, Insta_client_secret, Insta_Endpoint, Insta_Auth_Endpoint);

                // Get transaciton data using transactionId
                PaymentOrderDetailsResponse response = objClass.getPaymentOrderDetailsByTransactionId(orderId);
               
                // get transaciton data using orderId
                PaymentOrderDetailsResponse response1 = objClass.getPaymentOrderDetails(id);
                 
                PaymentRequest pay = new PaymentRequest()
                {
                    UserId = Convert.ToString(user.id),
                    Amount = Convert.ToDecimal(response.amount),
                    OrderID = orderId,
                    TID = uniqueRefID,
                    TDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    Status = status,
                    UserName = Convert.ToString(user.username),
                    ID= id
                };
                var statusID = await this.objRepository.SavePaymentRequest(pay);
                if (response.status.ToLower().Equals("completed") == true)
                {
                    Dashboard detailModel  = new Dashboard();
                    objRepository = new APIRepository();
                    var result = new Response();
                    detailModel.OrderDetail = new order();


                    detailModel.OrderDetail.id = Convert.ToInt32(Session["OrderId"]);
                    detailModel.OrderDetail.payment_ref_no = uniqueRefID;
                    detailModel.OrderDetail.id = Session["OrderId"] != null ? Convert.ToInt32(Session["OrderId"]) : 0;
                    detailModel.OrderDetail.payment_ref_amount = Convert.ToString(response.amount);
                    detailModel.OrderDetail.user_id = user.id;
                    result = await objRepository.CreateOrder(detailModel.OrderDetail, "Paymentgateway");//editwithpoints
                    ViewBag.Message = " Payment Done Sucessfully.!";
                    return RedirectToAction("thankYouPage","Manage");
                }
                else
                {
                    ViewBag.Message = " Payment Failed,Please try Again letter!";
                    return RedirectToAction("Failed", "Home");
                }
            }
             catch(Exception ex)
            {

            }
            return RedirectToAction("Failed", "Home");
        }
        public async Task<ActionResult> Failed()
        {
            Dashboard data = new Dashboard();
            data.OrderDetail = new order();
            data.OrderDetail.id = Session["OrderId"] != null ? Convert.ToInt32(Session["OrderId"]) : 0;
            ViewBag.Message = " Payment Failed,Please try Again letter!";
            return View(data);
        }


      
        public async Task<ActionResult> generateToken(string Amount,string cmpId)
        {
            string response = string.Empty;
            try
            {
                HttpWebRequest reponse;
                string URL = string.Empty;
                PaymentTokenReq paytoken = new PaymentTokenReq();
                paytoken.amount = Amount;
                paytoken.contact_number = "";
                paytoken.email_id = "";
                paytoken.currency = "INR";
                paytoken.isSandBox = false;
                paytoken.companyId = cmpId;
                string output1 = JsonConvert.SerializeObject(paytoken);
                reponse = clsgen.PostJSON(output1, "http://uapi.bisplindia.in/api/Home/GeneratePaymentTokenByCompanyId");
                response = clsgen.GetResponse(reponse);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
            //return response;
            return Json(response);
        }

         public async Task<ActionResult> GenerateTokenHealthStanvee(string Amount)
        {
            string response = string.Empty;
            try
            {
                HttpWebRequest reponse;
                string URL = string.Empty;
                PaymentTokenReq paytoken = new PaymentTokenReq();
                paytoken.amount = Amount;
                paytoken.contact_number = "";
                paytoken.email_id = "";
                paytoken.currency = "INR";
                string output1 = JsonConvert.SerializeObject(paytoken);
                reponse = clsgen.PostJSON(output1, "http://uapi.bisplindia.in/api/Home/GeneratePaymentTokenNew");
                response = clsgen.GetResponse(reponse);
            }
             catch(Exception ex)
            {

            }
            return Json(response);
        }

         public async Task<ActionResult> SaveRequest(string Amount,string paymentToken)
        {
            string urm = "";
            if (companyId == "30" || companyId=="46")
            {
                var user = Session["UserDetail"] as UserDetails;
                string agentid = "";
                agentid = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                paymentRequestGoHappy paymnt = new paymentRequestGoHappy()
                {
                    UserId = Convert.ToString(user.id),
                    Amount = Convert.ToDecimal(Amount),
                    OrderNo = paymentToken,
                    TID = "",
                    TDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    Status = "",
                    UserName = Convert.ToString(user.username)
                };
                DataSet ds = null;
                ds = new DataSet();
                var statusID = await this.objRepository.SavePaymentGohappyRequest(paymnt);

            }
            return Json(urm);
        }
         public async Task<ActionResult> SaveResponse(string paymentToken, string paymentId, string status, string amount, string response)
        {
            string output2 = string.Empty;
            var user = Session["UserDetail"] as UserDetails;
            string agentid = "";
            agentid = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string url = string.Empty;
            try
            {
                paymentRequestGoHappy pay = new paymentRequestGoHappy()
                {
                    UserId = Convert.ToString(user.id),
                    Amount = Convert.ToDecimal(amount),
                    OrderNo = paymentToken,
                    TID = paymentId,
                    TDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    Status = status,
                    UserName = Convert.ToString(user.username),
                    Response = response

                };
                var statusID = await this.objRepository.SavePaymentGohappyRequest(pay);
                 if(status.ToUpper() == "CAPTURED")
                {
                    Dashboard detailModel = new Dashboard();
                    objRepository = new APIRepository();
                    var result = new Response();
                    detailModel.OrderDetail = new order();


                    detailModel.OrderDetail.id = Convert.ToInt32(Session["OrderId"]);
                    detailModel.OrderDetail.payment_ref_no = paymentId;
                    detailModel.OrderDetail.id = Session["OrderId"] != null ? Convert.ToInt32(Session["OrderId"]) : 0;
                    detailModel.OrderDetail.payment_ref_amount = Convert.ToString(amount);
                    detailModel.OrderDetail.user_id = user.id;
                    result = await objRepository.CreateOrder(detailModel.OrderDetail, "Paymentgateway");//editwithpoints
                    ViewBag.Message = " Payment Done Sucessfully.!";
                    url = "/Manage/thankYouPage";
                    return Json(url);
                   // return RedirectToAction("thankYouPage", "Manage");
                }
                else
                {
                    ViewBag.Message = " Payment Failed,Please try Again letter!";
                    url = "/Home/Failed";
                    return Json(url);
                    //return RedirectToAction("Failed", "Home");
                }
            }
             catch(Exception ex)
            {

            }
            return Json(url);
            //  return RedirectToAction("Failed", "Home");
        }

    }
}