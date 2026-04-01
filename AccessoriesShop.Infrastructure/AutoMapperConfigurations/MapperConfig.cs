using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;
using AccessoriesShop.Domain.Entities;
using AutoMapper;

namespace AccessoriesShop.Infrastructure.AutoMapperConfigurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            // Account
            CreateMap<RegisterRequest, Account>().ReverseMap();
            CreateMap<UpdateAccountRequest, Account>().ReverseMap();
            CreateMap<Account, AccountResponse>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
                .ReverseMap();

            // Attributes
            CreateMap<CreateAttributesRequest, Attributes>().ReverseMap();
            CreateMap<Attributes, AttributesResponse>().ReverseMap();

            // Brand
            CreateMap<CreateBrandRequest, Brand>().ReverseMap();
            CreateMap<Brand, BrandResponse>().ReverseMap();

            // Category
            CreateMap<CreateCategoryRequest, Category>().ReverseMap();
            CreateMap<Category, CategoryResponse>().ReverseMap();

            // Device
            CreateMap<CreateDeviceRequest, Device>().ReverseMap();
            CreateMap<Device, DeviceResponse>().ReverseMap();

            // ProductCompatibility
            CreateMap<CreateProductCompatibilityRequest, ProductCompatibility>().ReverseMap();
            CreateMap<ProductCompatibility, ProductCompatibilityResponse>().ReverseMap();

            // ProductAttribute
            CreateMap<CreateProductAttributeRequest, ProductAttribute>().ReverseMap();
            CreateMap<ProductAttribute, ProductAttributeResponse>().ReverseMap();

            // ProductVariant
            CreateMap<CreateProductVariantRequest, ProductVariant>().ReverseMap();
            CreateMap<ProductVariant, ProductVariantResponse>().ReverseMap();

            // Product
            CreateMap<CreateProductRequest, Product>().ReverseMap();
            CreateMap<Product, ProductResponse>().ReverseMap();

            //Order
            CreateMap<CreateOrderRequest, Order>()
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => new List<OrderItem>()))
                .ReverseMap();
            CreateMap<CreateOrderItemRequest, OrderItem>().ReverseMap();
            CreateMap<OrderItem, OrderItemResponse>().ReverseMap();
            CreateMap<Order, OrderResponse>().ReverseMap();

            // Payment
            CreateMap<CreatePaymentRequest, Payment>().ReverseMap();
            CreateMap<Payment, PaymentResponse>().ReverseMap();

            // CartItem
            CreateMap<CartItemRequest, CartItem>().ReverseMap();
            CreateMap<CartItem, CartItemResponse>().ReverseMap(); 
            CreateMap<Cart, CartResponse>().ReverseMap();
            // Custom Order
            CreateMap<CreateCustomOrderRequest, CustomOrder>()
                .ForMember(dest => dest.ProductBaseId, opt => opt.Ignore())
                .ForMember(dest => dest.Files, opt => opt.Ignore()).ReverseMap();
            CreateMap<CustomOrderFile, CustomOrderFileResponse>().ReverseMap();
            CreateMap<CustomOrder, CustomOrderResponse>().ReverseMap();
			// Rating
			CreateMap<CreateRatingRequest, Rating>().ReverseMap();
			CreateMap<UpdateRatingRequest, Rating>().ReverseMap();
			CreateMap<Rating, RatingResponse>().ReverseMap();
			// Promotion
			CreateMap<CreatePromotionRequest, Promotion>().ReverseMap();
			CreateMap<UpdatePromotionRequest, Promotion>().ReverseMap();
			CreateMap<Promotion, PromotionResponse>().ReverseMap();


            // Address
            CreateMap<CreateAddressRequest, Address>().ReverseMap();
            CreateMap<Address, UserAddressResponse>().ReverseMap();
        }
    }
}
