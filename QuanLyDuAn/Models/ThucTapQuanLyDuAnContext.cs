using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace QuanLyDuAn.Models;

public partial class ThucTapQuanLyDuAnContext : DbContext
{
    public ThucTapQuanLyDuAnContext()
    {
    }

    public ThucTapQuanLyDuAnContext(DbContextOptions<ThucTapQuanLyDuAnContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CapNhatCongViec> CapNhatCongViecs { get; set; }

    public virtual DbSet<CongViec> CongViecs { get; set; }

    public virtual DbSet<DuAn> DuAns { get; set; }

    public virtual DbSet<Kpi> Kpis { get; set; }

    public virtual DbSet<Luong> Luongs { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<NhanVienThamGiaDuAn> NhanVienThamGiaDuAns { get; set; }

    public virtual DbSet<PhanCongCongViec> PhanCongCongViecs { get; set; }

    public virtual DbSet<Quyen> Quyens { get; set; }
    public virtual DbSet<ThongBao> ThongBaos { get; set; }
    public virtual DbSet<TrangThai> TrangThais { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["QLDAConnection"].ConnectionString);
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CapNhatCongViec>(entity =>
        {
            entity.HasKey(e => new { e.CnId, e.CvId, e.DaId, e.NvId });
            entity.ToTable("CapNhatCongViec", tb => tb.HasTrigger("trg_CapNhatCongViec_ThemThongBao"));
            entity.ToTable("CapNhatCongViec");
            entity.Property(e => e.CnId)
                .ValueGeneratedOnAdd()
                .HasColumnName("cn_ID");
            entity.Property(e => e.CvId).HasColumnName("cv_ID");
            entity.Property(e => e.DaId).HasColumnName("da_ID");
            entity.Property(e => e.NvId).HasColumnName("nv_ID");
            entity.Property(e => e.CnFile)
                .HasMaxLength(50)
                .HasDefaultValue("")
                .HasColumnName("cn_File");
            entity.Property(e => e.CnMoTa)
                .HasMaxLength(100)
                .HasColumnName("cn_MoTa");
            entity.Property(e => e.CnThoiGian)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("cn_ThoiGian");
            entity.Property(e => e.CvPath)
                .HasMaxLength(500)
                .HasDefaultValue("")
                .HasColumnName("cv_Path");

            entity.HasOne(d => d.PhanCongCongViec).WithMany(p => p.CapNhatCongViecs)
                .HasForeignKey(d => new { d.CvId, d.DaId, d.NvId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CapNhatCongViec_PhanCongCongViec");
        });

        modelBuilder.Entity<CongViec>(entity =>
        {
            entity.HasKey(e => new { e.CvId, e.DaId });

            entity.ToTable("CongViec", tb =>
            {
                tb.HasTrigger("TRG_DeleteCongViec");
                tb.HasTrigger("TRG_UpdateKPI");
                tb.HasTrigger("TRG_UpdateTienDoDuAn");
            });

            entity.Property(e => e.CvId)
                .ValueGeneratedOnAdd()
                .HasColumnName("cv_ID");
            entity.Property(e => e.DaId).HasColumnName("da_ID");
            entity.Property(e => e.CvBatDau)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("cv_BatDau");
            entity.Property(e => e.CvFile)
                .HasMaxLength(50)
                .HasDefaultValue("")
                .HasColumnName("cv_File");
            entity.Property(e => e.CvKetThuc)
                .HasDefaultValueSql("('')")
                .HasColumnName("cv_KetThuc");
            entity.Property(e => e.CvMa)
                .HasMaxLength(17)
                .IsUnicode(false)
                .HasComputedColumnSql("('CV'+CONVERT([varchar](15),[cv_ID]))", true)
                .HasColumnName("cv_Ma");
            entity.Property(e => e.CvMoTa)
                .HasMaxLength(200)
                .HasDefaultValue("")
                .HasColumnName("cv_MoTa");
            entity.Property(e => e.CvPath)
                .HasMaxLength(500)
                .HasDefaultValue("")
                .HasColumnName("cv_Path");
            entity.Property(e => e.CvTen)
                .HasMaxLength(50)
                .HasColumnName("cv_Ten");
            entity.Property(e => e.CvThoiGianHoanThanh)
                .HasDefaultValueSql("('')")
                .HasColumnName("cv_ThoiGianHoanThanh");
            entity.Property(e => e.NvIdNguoiTao).HasColumnName("nv_ID_NguoiTao");
            entity.Property(e => e.TtMa)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("tt_Ma");

            entity.HasOne(d => d.TtMaNavigation).WithMany(p => p.CongViecs)
                .HasForeignKey(d => d.TtMa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CongViec_TrangThai");

            entity.HasOne(d => d.NhanVienThamGiaDuAn).WithMany(p => p.CongViecs)
                .HasForeignKey(d => new { d.NvIdNguoiTao, d.DaId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CongViec_NhanVienThanGiaDuAn");
        });

        modelBuilder.Entity<DuAn>(entity =>
        {
            entity.HasKey(e => e.DaId);

            entity.ToTable("DuAn", tb => tb.HasTrigger("TRG_DeleteDuAn"));

            entity.Property(e => e.DaId).HasColumnName("da_ID");
            entity.Property(e => e.DaBatDau)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("da_BatDau");
            entity.Property(e => e.DaFile)
                .HasMaxLength(50)
                .HasDefaultValue("")
                .HasColumnName("da_File");
            entity.Property(e => e.DaKetThuc).HasColumnName("da_KetThuc");
            entity.Property(e => e.DaMa)
                .HasMaxLength(17)
                .IsUnicode(false)
                .HasComputedColumnSql("('DA'+CONVERT([varchar](15),[da_ID]))", true)
                .HasColumnName("da_Ma");
            entity.Property(e => e.DaMoTa)
                .HasMaxLength(200)
                .HasDefaultValue("")
                .HasColumnName("da_MoTa");
            entity.Property(e => e.DaPath)
                .HasMaxLength(500)
                .HasDefaultValue("")
                .HasColumnName("da_Path");
            entity.Property(e => e.DaTen)
                .HasMaxLength(100)
                .HasColumnName("da_Ten");
            entity.Property(e => e.DaThoiGianHoanThanh)
                .HasDefaultValueSql("('')")
                .HasColumnName("da_ThoiGianHoanThanh");
            entity.Property(e => e.DaTienDo)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("da_TienDo");
            entity.Property(e => e.NvIdNguoiTao).HasColumnName("nv_ID_NguoiTao");
            entity.Property(e => e.TtMa)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("tt_Ma");

            entity.HasOne(d => d.NvIdNguoiTaoNavigation).WithMany(p => p.DuAns)
                .HasForeignKey(d => d.NvIdNguoiTao)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DuAn_NhanVien");

            entity.HasOne(d => d.TtMaNavigation).WithMany(p => p.DuAns)
                .HasForeignKey(d => d.TtMa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DuAn_TrangThai");
        });

        modelBuilder.Entity<Kpi>(entity =>
        {
            entity.HasKey(e => new { e.NvId, e.KpiThangNam });

            entity.ToTable("KPI", tb => tb.HasTrigger("TRG_InsertUpdateLuongFromKPI"));

            entity.Property(e => e.NvId).HasColumnName("nv_ID");
            entity.Property(e => e.KpiThangNam).HasColumnName("kpi_ThangNam");
            entity.Property(e => e.KpiPhanTram)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("kpi_PhanTram");

            entity.HasOne(d => d.Nv).WithMany(p => p.Kpis)
                .HasForeignKey(d => d.NvId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KPI_NhanVien");
        });

        modelBuilder.Entity<Luong>(entity =>
        {
            entity.HasKey(e => new { e.NvId, e.LuongThangNam });

            entity.ToTable("Luong");

            entity.Property(e => e.NvId).HasColumnName("nv_ID");
            entity.Property(e => e.LuongThangNam).HasColumnName("luong_ThangNam");
            entity.Property(e => e.LuongPhuCap)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("luong_PhuCap");
            entity.Property(e => e.LuongThucNhan)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("luong_ThucNhan");

            entity.HasOne(d => d.Nv).WithMany(p => p.Luongs)
                .HasForeignKey(d => d.NvId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Luong_NhanVien");
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.NvId);

            entity.ToTable("NhanVien", tb => tb.HasTrigger("TRG_DeleteNhanVien"));

            entity.HasIndex(e => e.NvTaiKhoan, "UQ__NhanVien__3360C9D2DBE34436").IsUnique();
            entity.HasIndex(e => e.NvTaiKhoan, "UQ__NhanVien__3360C9D26C863E77").IsUnique();

            entity.Property(e => e.NvId).HasColumnName("nv_ID");
            entity.Property(e => e.NvDiaChi)
                .HasMaxLength(100)
                .HasColumnName("nv_DiaChi");
            entity.Property(e => e.NvEmail)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nv_Email");
            entity.Property(e => e.NvGioiTinh)
                .HasMaxLength(5)
                .HasColumnName("nv_GioiTinh");
            entity.Property(e => e.NvLuongCoBan)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("nv_LuongCoBan");
            entity.Property(e => e.NvMa)
                .HasMaxLength(17)
                .IsUnicode(false)
                .HasComputedColumnSql("('NV'+CONVERT([varchar](15),[nv_ID]))", true)
                .HasColumnName("nv_Ma");
            entity.Property(e => e.NvMatKhau)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("nv_MatKhau");
            entity.Property(e => e.NvNgaySinh).HasColumnName("nv_NgaySinh");
            entity.Property(e => e.NvSdt)
                .HasMaxLength(15)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("nv_SDT");
            entity.Property(e => e.NvTaiKhoan)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nv_TaiKhoan");
            entity.Property(e => e.NvTen)
                .HasMaxLength(100)
                .HasColumnName("nv_Ten");
            entity.Property(e => e.QMa)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("q_Ma");

            entity.HasOne(d => d.QMaNavigation).WithMany(p => p.NhanViens)
                .HasForeignKey(d => d.QMa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NhanVien_Quyen");
        });

        modelBuilder.Entity<NhanVienThamGiaDuAn>(entity =>
        {
            entity.HasKey(e => new { e.NvId, e.DaId });

            entity.ToTable("NhanVienThamGiaDuAn", tb => tb.HasTrigger("TRG_DeleteNVThamGiaDA"));

            entity.Property(e => e.NvId).HasColumnName("nv_ID");
            entity.Property(e => e.DaId).HasColumnName("da_ID");
            entity.Property(e => e.VtMa)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("vt_Ma");

            entity.HasOne(d => d.Da).WithMany(p => p.NhanVienThamGiaDuAns)
                .HasForeignKey(d => d.DaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NhanVienThamGiaDuAn_DuAn");

            entity.HasOne(d => d.Nv).WithMany(p => p.NhanVienThamGiaDuAns)
                .HasForeignKey(d => d.NvId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NhanVienThamGiaDuAn_NhanVien");

            entity.HasOne(d => d.VtMaNavigation).WithMany(p => p.NhanVienThamGiaDuAns)
                .HasForeignKey(d => d.VtMa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NhanVienThamGiaDuAn_VaiTro");
        });

        modelBuilder.Entity<PhanCongCongViec>(entity =>
        {
        entity.HasKey(e => new { e.CvId, e.DaId, e.NvId });

        entity.ToTable("PhanCongCongViec", tb =>
        {
            tb.HasTrigger("TRG_DeletePhanCongCongViec");
            tb.HasTrigger("TRG_UpdateKPI_PhanCong");
            tb.HasTrigger("trg_PhanCongCongViec_ThemThongBao");
        });

            entity.HasIndex(e => e.CvId, "UQ__PhanCong__C36B8FFFC2955FA2").IsUnique();

            entity.Property(e => e.CvId).HasColumnName("cv_ID");
            entity.Property(e => e.DaId).HasColumnName("da_ID");
            entity.Property(e => e.NvId).HasColumnName("nv_ID");

            entity.HasOne(d => d.CongViec).WithMany(p => p.PhanCongCongViecs)
                .HasForeignKey(d => new { d.CvId, d.DaId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PhanCongCongViec_CongViec");

            entity.HasOne(d => d.NhanVienThamGiaDuAn).WithMany(p => p.PhanCongCongViecs)
                .HasForeignKey(d => new { d.NvId, d.DaId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PhanCongCongViec_NhanVienThamGiaDuAn");
        });

        modelBuilder.Entity<Quyen>(entity =>
        {
            entity.HasKey(e => e.QMa);

            entity.ToTable("Quyen");

            entity.Property(e => e.QMa)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("q_Ma");
            entity.Property(e => e.QMoTa)
                .HasMaxLength(50)
                .HasDefaultValue("")
                .HasColumnName("q_MoTa");
            entity.Property(e => e.QTen)
                .HasMaxLength(50)
                .HasColumnName("q_Ten");
        });

        modelBuilder.Entity<ThongBao>(entity =>
        {
            entity.HasKey(e => e.TbId);

            entity.ToTable("ThongBao");

            entity.Property(e => e.TbId).HasColumnName("tb_ID");
            entity.Property(e => e.CvId).HasColumnName("cv_ID");
            entity.Property(e => e.DaId).HasColumnName("da_ID");
            entity.Property(e => e.NvIdNguoiGui).HasColumnName("nv_ID_NguoiGui");
            entity.Property(e => e.NvIdNguoiNhan).HasColumnName("nv_ID_NguoiNhan");
            entity.Property(e => e.TbLoai)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tb_Loai");
            entity.Property(e => e.TbNoiDung)
                .HasMaxLength(200)
                .HasColumnName("tb_NoiDung");
            entity.Property(e => e.TbThoiGian)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("tb_ThoiGian");
            entity.Property(e => e.TbTinhTrang)
                .HasDefaultValue(false)
                .HasColumnName("tb_TinhTrang");

            // Cấu hình mối quan hệ với NhanVien (NvIdNguoiGui)
            entity.HasOne(d => d.NvIdNguoiGuiNavigation)
                  .WithMany(p => p.ThongBaoNvIdNguoiGuiNavigations)
                  .HasForeignKey(d => d.NvIdNguoiGui)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_ThongBao_NhanVien_NguoiGui");

            // Cấu hình mối quan hệ với NhanVien (NvIdNguoiNhan)
            entity.HasOne(d => d.NvIdNguoiNhanNavigation)
                  .WithMany(p => p.ThongBaoNvIdNguoiNhanNavigations)
                  .HasForeignKey(d => d.NvIdNguoiNhan)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_ThongBao_NhanVien_NguoiNhan");

            // Cấu hình mối quan hệ với CongViec
            entity.HasOne(d => d.CongViec)
                  .WithMany(p => p.ThongBaos)
                  .HasForeignKey(d => new { d.CvId, d.DaId })
                  .HasConstraintName("FK_ThongBao_CongViec");
        });
        modelBuilder.Entity<TrangThai>(entity =>
        {
            entity.HasKey(e => e.TtMa);

            entity.ToTable("TrangThai");

            entity.Property(e => e.TtMa)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("tt_Ma");
            entity.Property(e => e.TtTen)
                .HasMaxLength(50)
                .HasColumnName("tt_Ten");
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.VtMa);

            entity.ToTable("VaiTro");

            entity.Property(e => e.VtMa)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("vt_Ma");
            entity.Property(e => e.VtMoTa)
                .HasMaxLength(50)
                .HasDefaultValue("")
                .HasColumnName("vt_MoTa");
            entity.Property(e => e.VtTen)
                .HasMaxLength(50)
                .HasColumnName("vt_Ten");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
