using AutoMapper;
using OMS.Data.Entites;
using OMS.Repositores.DTO;



namespace OrderMangmentSystem.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Order, OrderDTO>()

            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.CreatedTime))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Customer.Email))
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));
            
            CreateMap<OrderItem, OrderItemDTO>().ReverseMap();
            

            CreateMap<Order, CreateOrderDTO>().ReverseMap();
            CreateMap<CreateCustomerDTO, Customer>().ReverseMap();
            CreateMap<Customer, CustomerDTO>().ReverseMap();
            CreateMap<Invoice, InvoiceDTO>().ReverseMap();
            CreateMap<addNewProductDTO, Product>().ReverseMap();
            CreateMap<ProductDTO, Product>().ReverseMap();

        }
    }

}
