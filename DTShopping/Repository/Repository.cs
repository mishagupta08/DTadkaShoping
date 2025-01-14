﻿using DTShopping.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DTShopping.Repository
{
    public class APIRepository
    {
        int ETRADECOMPANYID = 42;

        private int RoleId = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["RoleId"]);

        private int CompanyId = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["CompanyId"]);

        private string ApiUrl = System.Configuration.ConfigurationManager.AppSettings["ApiUrl"];

        private string GetGetShoppingPortalAllFrontPageProductsListAction = "GetShoppingPortalAllFrontPageProductsList/";

        private string ManagePointsLedgerAction = "ManagePointsLedger/";

        private string ManageListWithFilterAction = "ManageListWithFilter";

        private string ManageProductImagesAction = "ManageProductImages/";

        private string DeliveryTypeListAction = "DeliveryTypeList/";

        private string GetCouponListAction = "GetCouponList/";

        private string ManageProductsAction = "ManageProducts/";

        private string MangeOtpFunctionsAction = "MangeOtpFunctions/";

        private string GetUserPointsByUserIdAction = "GetUserPointsByUserId/";

        private string ManageCartAction = "ManageCart/";
        private string ManageHWallet = "ManageHWallet/";

        private string ManageCompaniesAction = "ManageCompany/";

        private string ManageShoppingProductsListWithFilter = "ManageShoppingProductsListWithFilter/";

        private string ManageCategory = "CategoryDetail/";

        private string ManageOrderAction = "ManageOrder/";

        private string ManageCompanyProfileAction = "ManageCompanyProfile/";

        private string ManageDealProducts = "GetDealProductsFullList/";

        private string ManageOrderproducts = "ManageVendorProductOrderListWithFilter/";

        private string ManageOrder = "ManageOrder/";

        private string GetProductImagesListAction = "ManageProductImages/ListByProductId";

        private string ManagePointLedgerAction = "ManagePointsLedger/";

        public async Task<Response> ManageCompanyProfile(CompanyProfile orderData, string action)
        {
            var data = JsonConvert.SerializeObject(orderData);
            var result = await CallPostFunction(data, ManageCompanyProfileAction + action);
            if (result == null)
            {
                return null;
            }
            else
            {
                return result;
            }
        }


        public async Task<List<discount_coupons>> GetCouponList(int userId)
        {
            var result = await CallPostFunction(string.Empty, GetCouponListAction + userId);
            if (result == null || !result.Status)
            {
                return null;
            }
            else
            {
                var list = JsonConvert.DeserializeObject<List<discount_coupons>>(result.ResponseValue);
                return list;
            }
        }

        public async Task<List<KeyValue>> DeliveryTypeList( int companyId)
        {
            var result = await CallPostFunction(string.Empty, DeliveryTypeListAction + companyId);
            if (result == null || !result.Status)
            {
                return null;
            }
            else
            {
                var list = JsonConvert.DeserializeObject<List<KeyValue>>(result.ResponseValue);
                return list;
            }
        }

        public async Task<List<Category>> GetMenuList()
        {
            var cat = new List<Category>();
            cat.Add(new Category { CompanyId = CompanyId });
            var detail = JsonConvert.SerializeObject(cat);
            var result = await CallPostFunction(detail, "ManageCategories/CompanyCategoryList");
            if (result == null || !result.Status)
            {
                return null;
            }
            else
            {
                try
                {
                    var CategoryList = JsonConvert.DeserializeObject<List<Category>>(result.ResponseValue);
                    return CategoryList;
                }
                catch (Exception ex)
                {

                }
                return null;
            }
        }

        public async Task<company> GetCompanyById(List<company> companies)
        {
            var companyData = JsonConvert.SerializeObject(companies);
            var result = await CallPostFunction(companyData, ManageCompaniesAction + "ById");
            if (result == null || result.Status == false)
            {
                return null;
            }
            else
            {
                var detail = JsonConvert.DeserializeObject<company>(result.ResponseValue);
                return detail;
            }
        }

        public async Task<List<Banners>> GetBannerImageList(string Id)
        {
            var result = await CallPostFunction(string.Empty, "GetBannerImageList/" + Id);
            if (result == null || !result.Status)
            {
                return null;
            }
            else
            {
                var BannersList = JsonConvert.DeserializeObject<List<Banners>>(result.ResponseValue);
                return BannersList;
            }
        }

        public async Task<ShoppingPortalFrontPageProdList> GetShoppingPortalFrontPageProdList(string Id)
        {
            var result = await CallPostFunction(string.Empty, "GetShoppingPortalAllFrontPageProductsList/" + Id);
            if (result == null || !result.Status)
            {
                return null;
            }
            else
            {
                var HomePageSections = JsonConvert.DeserializeObject<ShoppingPortalFrontPageProdList>(result.ResponseValue);
                return HomePageSections;
            }
        }

        public async Task<Response> Login(UserDetails user)
        {
            //if (user.company_id == ETRADECOMPANYID)
            //{
            //    user.passwordDetail = Base64Encode(user.passwordDetail);
            //    user.password_str = user.passwordDetail;
            //}

            var detail = JsonConvert.SerializeObject(user);
            var result = await CallPostFunction(detail, "LoginShoppigPortalUser");
            if (result != null)
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public async Task<ShoppingPortalFrontPageProdList> GetGetShoppingPortalAllFrontPageProductsList()
        {
            var result = await CallPostFunction(string.Empty, GetGetShoppingPortalAllFrontPageProductsListAction + CompanyId);
            if (result == null)
            {
                return null;
            }
            else
            {
                var productList = JsonConvert.DeserializeObject<ShoppingPortalFrontPageProdList>(result.ResponseValue);
                return productList;
            }
        }

        public async Task<Response> Register(UserDetails user)
        {
            user.role_id = RoleId;
            user.company_id = CompanyId;
            var list = new List<UserDetails>();
            list.Add(user);
            var detail = JsonConvert.SerializeObject(list);
            var result = await CallPostFunction(detail, "ManageRetailer/Add");

            //var result = JsonConvert.DeserializeObject<Response>(result.ResponseValue);
            return result;

        }

        public async Task<List<R_StateMaster>> GetStateList()
        {
            var result = await CallPostFunction(string.Empty, "StateList");
            if (result == null || !result.Status)
            {
                return null;
            }
            else
            {
                var states = JsonConvert.DeserializeObject<List<R_StateMaster>>(result.ResponseValue);
                return states;
            }
        }

        public async Task<List<AreaCoderesponse>> GetAreaCode(string PinCode)
        {
            var result = await CallPostFunction(string.Empty, "AreaList/" + PinCode);
            //var result = await CallPostFunction(string.Empty, "AreaList");
            if (result == null || !result.Status)
            {
                return null;
            }
            else
            {
                var Area = JsonConvert.DeserializeObject<List<AreaCoderesponse>>(result.ResponseValue);
                return Area;
            }
        }
          
       


        public async Task<Response> SaveAPIRequest(ApiPinCoderesponse code)
        {
            var filterData = JsonConvert.SerializeObject(code);
            var result = await CallPostFunction(filterData, "SaveAPIRequest" );
       
            //var result = await CallPostFunction(string.Empty, "AreaList");
            if (result == null || !result.Status)
            {
                return null;
            }
            else
            {
                var Area = result.ResponseValue;
                //var Area = JsonConvert.DeserializeObject<Response>(result.ResponseValue);
                return result;
            }
        }

         public async Task<Response> SavePaymentRequest(PaymentRequest regXML)
        {
            var filterData = JsonConvert.SerializeObject(regXML);
            var result = await CallPostFunction(filterData, "InstaMojoPayment");
            if(result == null || !result.Status)
            {
                return null;
            }
            else
            {
                var response = result.ResponseValue;
                return result;
            }
        }
         public async Task<Response> SavePaymentGohappyRequest(paymentRequestGoHappy regxml)
        {
            var filterData = JsonConvert.SerializeObject(regxml);
            var result = await CallPostFunction(filterData, "GohappyPaymentGetway");
            {
                if(result==null||!result.Status)
                {
                    return null;
                }
                else
                {
                    var response = result.ResponseValue;
                    return result;
                }
            }
        }

        public async Task<List<R_CityMaster>> GetCityListById(string Id)
        {
            var result = await CallPostFunction(string.Empty, "CityList/" + Id);
            if (result == null || !result.Status)
            {
                return null;
            }
            else
            {
                var cities = JsonConvert.DeserializeObject<List<R_CityMaster>>(result.ResponseValue);
                return cities;
            }
        }

        public async Task<Response> ManageCart(CartFilter filter, string action)
        {
            filter.companyId = CompanyId;
            var filterData = JsonConvert.SerializeObject(filter);
            var result = await CallPostFunction(filterData, ManageCartAction + action);
            if (result == null)
            {
                return null;
            }
            else
            {
                return result;
            }
        }

        public async Task<Response> ManagePointLedger(PointsLedger filter, string action)
        {
            var filterData = JsonConvert.SerializeObject(filter);
            var result = await CallPostFunction(filterData, ManagePointLedgerAction + action);
            if (result == null)
            {
                return null;
            }
            else
            {
                return result;
            }
        }



        public async Task<Response> GetProductImage(int prodId)
        {
            var data = new product_images();
            data.product_id = prodId;
            var d = JsonConvert.SerializeObject(data);
            var result = await CallPostFunction(d, ManageProductImagesAction + "List");
            if (result == null)
            {
                return null;
            }
            else
            {
                return result;
            }
        }

        public async Task<ProductDetailOrangeThemeContainer> GetProductDetailByIdForOrangeTheme(List<Product> products)
        {
            products.FirstOrDefault().companyId = CompanyId;
            var productData = JsonConvert.SerializeObject(products);
            var result = await CallPostFunction(productData, ManageProductsAction + "ByIdForOrangeTheme");
            if (result == null || !result.Status)
            {
                return null;
            }
            else
            {
                var prod = JsonConvert.DeserializeObject<ProductDetailOrangeThemeContainer>(result.ResponseValue);
                return prod;
            }
        }

        public async Task<Product> GetProductDetailById(List<Product> products)
        {
            products.FirstOrDefault().companyId = CompanyId;
            var productData = JsonConvert.SerializeObject(products);
            var result = await CallPostFunction(productData, ManageProductsAction + "ById");
            if (result == null || !result.Status)
            {
                return null;
            }
            else
            {
                var prod = JsonConvert.DeserializeObject<Product>(result.ResponseValue);
                return prod;
            }
        }

        public async Task<Response> MangeOtpFunctions(UserDetails user, string operation)
        {
            user.company_id = CompanyId;
            var data = JsonConvert.SerializeObject(user);
            var result = await CallPostFunction(data, MangeOtpFunctionsAction + operation);
            return result;
        }

        public async Task<List<product_images>> GetProductImagesList(List<product_images> imageDetail)
        {
            var data = JsonConvert.SerializeObject(imageDetail);
            var result = await CallPostFunction(data, GetProductImagesListAction);
            if (result == null || !result.Status)
            {
                return null;
            }
            else
            {
                var list = JsonConvert.DeserializeObject<List<product_images>>(result.ResponseValue);
                return list;
            }
        }

        public async Task<Response> GetCategoryProducts(Filters FilterDetails)
        {
            FilterDetails.CompanyId = CompanyId;
            var productData = JsonConvert.SerializeObject(FilterDetails);
            var result = await CallPostFunction(productData, ManageShoppingProductsListWithFilter);
            if (result == null)
            {
                return null;
            }
            else
            {
                return result;
            }
        }

        public async Task<Category> GetCategoryDetail(Filters FilterDetails)
        {
            var productData = JsonConvert.SerializeObject(FilterDetails);
            var result = await CallPostFunction(productData, ManageCategory + FilterDetails.CategoryId);
            if (result == null)
            {
                return null;
            }
            else
            {
                var catDetail = JsonConvert.DeserializeObject<Category>(result.ResponseValue);
                return catDetail;
            }
        }

        public async Task<Response> ManageListWithFilter(Filters filter)
        {
            var detail = JsonConvert.SerializeObject(filter);
            var result = await CallPostFunction(detail, ManageListWithFilterAction);
            return result;
        }

        public async Task<Response> GetDealProductsFullList(Filters FilterDetails, string Deal)
        {
            var productData = JsonConvert.SerializeObject(FilterDetails);
            var result = await CallPostFunction(productData, ManageDealProducts + Deal);
            if (result == null)
            {
                return null;
            }
            else
            {
                return result;
            }
        }


        public async Task<Response> GetUserOrderList(Filters FilterDetails)
        {
            var productData = JsonConvert.SerializeObject(FilterDetails);
            var result = await CallPostFunction(productData, ManageOrderproducts);
            if (result == null)
            {
                return null;
            }
            else
            {
                return result;
            }
        }
        public async Task<Response> getCartCount(UserDetails userDetail)
        {
            CartFilter filter = new CartFilter();

            filter.username = userDetail.username;
            filter.password = userDetail.password_str;
            filter.userId = userDetail.id;
            filter.companyId = userDetail.company_id ?? 0;
            var productData = JsonConvert.SerializeObject(filter);
            var result = await CallPostFunction(productData, ManageCartAction + "CartCount");
            if (result == null)
            {
                return null;
            }
            else
            {
                return result;
            }
        }

         public async Task<Response> getHWalletBalance(UserDetails userdetail)
        {
                CartFilter filter = new CartFilter();
                filter.username = userdetail.username;
                filter.password = userdetail.password_str;
                filter.userId = userdetail.id;
                filter.companyId = userdetail.company_id ?? 0;
                var productData = JsonConvert.SerializeObject(filter);
               var result = await CallPostFunction(productData, ManageHWallet + "GetHWalletBalance");
                 if( result==null)
                {
                    return null;
                }
                else
                {
                    return result;
                }
            
           
            return result;
        }
        public async Task<order> GetUserOpenOrder(order orderDetail)
        {
            var productData = JsonConvert.SerializeObject(orderDetail);
            var result = await CallPostFunction(productData, "ManageOrder/UserOpenOrder");
            if (result == null)
            {
                return null;
            }
            else
            {
                return JsonConvert.DeserializeObject<order>(result.ResponseValue);
            }
        }

        public async Task<OrderDetailContainer> ManageOrderWithProducts(order orderData)
        {
            var data = JsonConvert.SerializeObject(orderData);
            var result = await CallPostFunction(data, ManageOrderAction + "ById");
            if (result == null)
            {
                return null;
            }
            else
            {
                return JsonConvert.DeserializeObject<OrderDetailContainer>(result.ResponseValue);
            }
        }

        public async Task<Dashboard> GetUserOrder(order orderDetail)
        {
            var productData = JsonConvert.SerializeObject(orderDetail);
            var result = await CallPostFunction(productData, "ManageOrder/ById");
            if (result == null)
            {
                return null;
            }
            else
            {
                return JsonConvert.DeserializeObject<Dashboard>(result.ResponseValue);
            }
        }

        public async Task<Response> CreateOrder(order orderDetail, string action)
        {
            var productData = JsonConvert.SerializeObject(orderDetail);
            var result = await CallPostFunction(productData, "ManageOrder/" + action);
            if (result == null)
            {
                return null;
            }
            else
            {
                return result;
            }
        }

         public async Task<Response> DeductHWallet(order orderDetail,string action)
        {
            var productData = JsonConvert.SerializeObject(orderDetail);
            var result = await CallPostFunction(productData, "DeductHWallet/" + action);
            if (result == null)
            {
                return null;
            }
            else
            {
                return result;
            }
        }

        private async Task<Response> CallPostFunction(string detail, string action)
        {
            using (var httpClient = new HttpClient())
            {
                // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                var httpContent = new StringContent(detail, Encoding.UTF8, "application/json");

                // Do the actual request and await the response
                var httpResponse = await httpClient.PostAsync(ApiUrl + action, httpContent);

                // If the response contains content we want to read it!
                if (httpResponse.Content != null)
                {
                    var responseContent = await httpResponse.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<Response>(responseContent);

                    return result;
                }
            }

            return null;
        }

        private async Task<Response> CallGetFunction(string action)
        {
            using (var httpClient = new HttpClient())
            {
                // Do the actual request and await the response
                var httpResponse = await httpClient.GetAsync(ApiUrl + action);

                // If the response contains content we want to read it!
                if (httpResponse.Content != null)
                {
                    var responseContent = await httpResponse.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<Response>(responseContent);

                    return result;
                }
            }

            return null;
        }

        public async Task<Response> UpdateAccount(UserDetails user)
        {
            user.role_id = RoleId;
            user.company_id = CompanyId;
            var detail = JsonConvert.SerializeObject(user);
            var result = await CallPostFunction(detail, "ManageVendor/Edit");
            return result;
        }

        public async Task<Response> CheckUserExistance(UserDetails user)
        {
            user.role_id = RoleId;
            user.company_id = CompanyId;
            var detail = JsonConvert.SerializeObject(user);
            var result = await CallPostFunction(detail, "ManageVendor/IsExists");
            return result;
        }

        public async Task<Response> CheckWalletBalance(UserDetails user)
        {
            user.company_id = CompanyId;
            var detail = JsonConvert.SerializeObject(user);
            var result = await CallPostFunction(detail, "CheckWalletAPI");
            return result;
        }

        public async Task<Response> DeductWalletBalnce(UserDetails user)
        {

            user.company_id = CompanyId;
            var detail = JsonConvert.SerializeObject(user);
            var result = await CallPostFunction(detail, "WalletDeductionAPI");
            return result;
        }

        public async Task<Response> WalletConfirmationAPI(UserDetails user)
        {

            user.company_id = CompanyId;
            var detail = JsonConvert.SerializeObject(user);
            var result = await CallPostFunction(detail, "WalletConfirmationAPI");
            return result;
        }

        public async Task<Response> ManagePoints(PointsLedger points, string action)
        {
            var data = JsonConvert.SerializeObject(points);
            var result = await CallPostFunction(data, ManagePointsLedgerAction + action);
            if (result == null)
            {
                return null;
            }
            else
            {
                return result;
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

    }
}