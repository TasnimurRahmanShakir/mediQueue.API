using AutoMapper;
using mediQueue.API.Model.DTO;
using mediQueue.API.Model.Entity;

namespace mediQueue.API.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {


            //====================
            // Patient Mapper
            //====================
            CreateMap<Patient, PatientDTO.Response>().ReverseMap();
            CreateMap<PatientDTO.Create, Patient>();
            CreateMap<PatientDTO.Update, Patient>();


            //====================
            // User Mapper
            //====================
            CreateMap<User, UserDTO.Response>().ReverseMap();
            CreateMap<UserDTO.Create, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
            CreateMap<User, UserDTO.Response>()
                .ForMember(dest => dest.DoctorProfile, opt => opt.MapFrom(src => src.DoctorProfile))
                .ForMember(dest => dest.ReceptionistProfile, opt => opt.MapFrom(src => src.ReceptionistProfile));


            //====================
            // Doctor Mapper
            //====================
            CreateMap<DoctorDTO.Create, Doctor>();
            CreateMap<DoctorDTO.Update, Doctor>();
            CreateMap<Doctor, DoctorDTO.Response>()
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.User.ImageUrl))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.User.Role))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email));


            //====================
            // Receptionist Mapper
            //====================
            CreateMap<ReceptionistDTO.Create, Receptionist>();
            CreateMap<Receptionist, ReceptionistDTO.Response>()
                .ForMember(dest => dest.ReceptionistName, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.User.ImageUrl))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.User.Role))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email));


            //====================
            // APPOINTMENT Mapper
            //====================
            CreateMap<AppointmentDTO.Create, Appointment>()
                .ForMember(dest => dest.Scedule, opt => opt.MapFrom(src => src.Schedule));
            CreateMap<Appointment, AppointmentDTO.Response>()
                .ForMember(dest => dest.Schedule, opt => opt.MapFrom(src => src.Scedule))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.User.Name))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.Name));


            //====================
            // Invoice Mapper
            //====================
            CreateMap<InvoiceDTO.Create, Invoice>();
            CreateMap<Invoice, InvoiceDTO.Response>();
        }
    }
}
