using AutoMapper;
using OMS.Data.Entites;
using OMS.Repositores.DTO;



namespace OrderMangmentSystem.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Order, OrderDTO>();
            CreateMap<OrderItem, OrderItemDTO>();
            CreateMap<CreateCustomerDTO, Customer>();
            CreateMap<Customer, CustomerDTO>();
            CreateMap<Invoice, InvoiceDTO>();

        }
    }

}
