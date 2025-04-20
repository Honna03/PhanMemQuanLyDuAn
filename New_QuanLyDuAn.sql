--------------------------------
-- Tạo cơ sở dữ liệu
CREATE DATABASE ThucTap
GO
USE ThucTap
GO

-- Bảng quyền
CREATE TABLE Quyen (
	q_Ma VARCHAR(15) NOT NULL,
	q_Ten NVARCHAR(50) NOT NULL,
	q_MoTa NVARCHAR(50) DEFAULT '',
	q_LuongCoBan DECIMAL(15,2) NOT NULL DEFAULT 0,
	--
	CONSTRAINT PK_Quyen PRIMARY KEY(q_Ma)
)
go
-- Bảng vai trò
CREATE TABLE VaiTro (
	vt_Ma VARCHAR(15) NOT NULL,
	vt_Ten NVARCHAR(50) NOT NULL,
	vt_MoTa NVARCHAR(50) DEFAULT '',
	--
	CONSTRAINT PK_VaiTro PRIMARY KEY(vt_Ma)
)
go
-- Bảng trạng thái
CREATE TABLE TrangThai (
	tt_Ma VARCHAR(15) NOT NULL,
	tt_Ten NVARCHAR(50) NOT NULL,
	--
	CONSTRAINT PK_TrangThai PRIMARY KEY(tt_Ma)
)
go
-- Bảng nhân viên
CREATE TABLE NhanVien (
	nv_ID INT IDENTITY(1,1),
	nv_Ma AS 'NV' + CAST(nv_ID AS VARCHAR(15)) PERSISTED,
	nv_Ten NVARCHAR(100) NOT NULL,
	nv_GioiTinh NVARCHAR(5) NOT NULL,
	nv_NgaySinh DATE NOT NULL,
	nv_SDT CHAR(15) NOT NULL CHECK (LEN(nv_SDT) = 10 AND nv_SDT LIKE '%[^0-9]%'),
	nv_DiaChi NVARCHAR(100) NOT NULL,
	nv_Email VARCHAR(100) NOT NULL,
	nv_TaiKhoan VARCHAR(50) UNIQUE NOT NULL,
	nv_MatKhau VARCHAR(256) NOT NULL,
	nv_LuongCoBan DECIMAL(15,2) DEFAULT 0 NOT NULL,
	--
	q_Ma VARCHAR(15) NOT NULL,
	--
	CONSTRAINT PK_NhanVien PRIMARY KEY(nv_ID),
	CONSTRAINT FK_NhanVien_Quyen FOREIGN KEY(q_Ma) REFERENCES Quyen(q_Ma)
)
go
CREATE TABLE KPI (
	kpi_ThangNam DATE NOT NULL, -- lưu đầu tháng để gom theo tháng
	kpi_PhanTram DECIMAL(5,2) DEFAULT 0,
	----
	nv_ID INT NOT NULL,
	CONSTRAINT PK_KPI PRIMARY KEY(nv_ID, kpi_ThangNam),
	CONSTRAINT FK_KPI_NhanVien FOREIGN KEY(nv_ID) REFERENCES NhanVien(nv_ID)
)
GO

CREATE TABLE Luong (
	luong_ThangNam DATE NOT NULL,
	luong_PhuCap DECIMAL(15,2) DEFAULT 0,
	luong_ThucNhan DECIMAL(15,2),
	--
	nv_ID INT NOT NULL,
	CONSTRAINT PK_Luong PRIMARY KEY(nv_ID, luong_ThangNam),
	CONSTRAINT FK_Luong_NhanVien FOREIGN KEY(nv_ID) REFERENCES NhanVien(nv_ID)
)
go

CREATE TABLE ThongTinCongTy (
    cty_ID INT IDENTITY(1,1),
    cty_Ten NVARCHAR(100) NOT NULL,
    cty_DiaChi NVARCHAR(200) NOT NULL,
    cty_SDT VARCHAR(30) NOT NULL,
    cty_Email VARCHAR(100) NULL CHECK (cty_Email LIKE '%@%.%'),
    cty_Logo NVARCHAR(500) NULL,
    cty_MoTa NVARCHAR(500) NULL,
    --
    CONSTRAINT PK_ThongTinCongTy PRIMARY KEY(cty_ID)
)
go

CREATE TABLE LichSuCapNhatLuong (
    ls_ID INT IDENTITY(1,1),
    q_Ma VARCHAR(15) NULL,
    nv_ID INT NULL,
    luongCu DECIMAL(15,2),
    luongMoi DECIMAL(15,2),
    thoiGianCapNhat DATETIME DEFAULT GETDATE(),
    --
    CONSTRAINT PK_LichSuCapNhatLuong PRIMARY KEY(ls_ID),
    CONSTRAINT FK_LichSuCapNhatLuong_Quyen FOREIGN KEY(q_Ma) REFERENCES Quyen(q_Ma),
    CONSTRAINT FK_LichSuCapNhatLuong_NhanVien FOREIGN KEY(nv_ID) REFERENCES NhanVien(nv_ID)
);
go
-- Bảng dự án
CREATE TABLE DuAn (
	da_ID INT IDENTITY(1,1),
	da_Ma AS 'DA' + CAST(da_ID AS VARCHAR(15)) PERSISTED,
	da_MoTa NVARCHAR(200) DEFAULT '',
	da_Ten NVARCHAR(100) NOT NULL,
	da_BatDau DATE DEFAULT GETDATE(),
	da_KetThuc DATE NOT NULL,
	da_File NVARCHAR(50) DEFAULT '',
	da_Path NVARCHAR(500) DEFAULT '',
	da_TienDo DECIMAL(5,2) NOT NULL DEFAULT 0 CHECK (da_TienDo >= 0 AND da_TienDo <= 100),
	da_ThoiGianHoanThanh DATE DEFAULT '',
	---
	nv_ID_NguoiTao INT NOT NULL,
	tt_Ma VARCHAR(15) NOT NULL,
	---
	CONSTRAINT PK_DuAn PRIMARY KEY(da_ID),
	CONSTRAINT FK_DuAn_NhanVien FOREIGN KEY(nv_ID_NguoiTao) REFERENCES NhanVien(nv_ID),
	CONSTRAINT FK_DuAn_TrangThai FOREIGN KEY(tt_Ma) REFERENCES TrangThai(tt_Ma)
)
go
-- Bảng nhân viên tham gia dự án
CREATE TABLE NhanVienThamGiaDuAn (
	nv_ID INT NOT NULL,
	da_ID INT NOT NULL,
	vt_Ma VARCHAR(15) NOT NULL,
	--
	CONSTRAINT PK_NhanVienThamGiaDuAn PRIMARY KEY(nv_ID, da_ID),
	CONSTRAINT FK_NhanVienThamGiaDuAn_NhanVien FOREIGN KEY(nv_ID) REFERENCES NhanVien(nv_ID),
	CONSTRAINT FK_NhanVienThamGiaDuAn_DuAn FOREIGN KEY(da_ID) REFERENCES DuAn(da_ID),
	CONSTRAINT FK_NhanVienThamGiaDuAn_VaiTro FOREIGN KEY(vt_Ma) REFERENCES VaiTro(vt_Ma)
)
go
-- Bảng công việc
CREATE TABLE CongViec (
	cv_ID INT IDENTITY(1,1),
	cv_Ma AS 'CV' + CAST(cv_ID AS VARCHAR(15)) PERSISTED,
	cv_Ten NVARCHAR(50) NOT NULL,
	cv_MoTa NVARCHAR(200) DEFAULT '',
	cv_BatDau DATE DEFAULT GETDATE(),
	cv_KetThuc DATE DEFAULT '',
	cv_File NVARCHAR(50) DEFAULT '',
	cv_Path NVARCHAR(500) DEFAULT '',
	cv_ThoiGianHoanThanh DATE DEFAULT '',
	---
	nv_ID_NguoiTao INT NOT NULL,
	da_ID INT NOT NULL,
	tt_Ma VARCHAR(15) NOT NULL,
	--
	CONSTRAINT PK_CongViec PRIMARY KEY(cv_ID, da_ID),
	CONSTRAINT FK_CongViec_NhanVienThanGiaDuAn FOREIGN KEY(nv_ID_NguoiTao, da_ID) REFERENCES NhanVienThamGiaDuAn(nv_ID, da_ID),
	CONSTRAINT FK_CongViec_TrangThai FOREIGN KEY(tt_Ma) REFERENCES TrangThai(tt_Ma)
)
go
-- Bảng phân công công việc
CREATE TABLE PhanCongCongViec (
	cv_ID INT UNIQUE NOT NULL,
	da_ID INT NOT NULL,
	nv_ID INT NOT NULL ,
	--
	CONSTRAINT PK_PhanCongCongViec PRIMARY KEY(cv_ID, da_ID, nv_ID),
	CONSTRAINT FK_PhanCongCongViec_CongViec FOREIGN KEY(cv_ID, da_ID) REFERENCES CongViec(cv_ID, da_ID),
	CONSTRAINT FK_PhanCongCongViec_NhanVienThamGiaDuAn FOREIGN KEY(nv_ID, da_ID) REFERENCES NhanVienThamGiaDuAn(nv_ID, da_ID)
)
GO

CREATE TABLE CapNhatCongViec(
	cn_ID INT IDENTITY(1,1),
	cn_MoTa NVARCHAR(100) NOT NULL,
	cn_File NVARCHAR(50) DEFAULT '',
	cv_Path NVARCHAR(500) DEFAULT '',
	cn_ThoiGian DATETIME DEFAULT GETDATE(),
	--------------
	cv_ID INT NOT NULL,
	da_ID INT NOT NULL,
	nv_ID INT NOT NULL,
	-------
	CONSTRAINT PK_CapNhatCongViec PRIMARY KEY(cn_ID, cv_ID, da_ID, nv_ID),
	CONSTRAINT FK_CapNhatCongViec_PhanCongCongViec FOREIGN KEY(cv_ID, da_ID, nv_ID) REFERENCES PhanCongCongViec(cv_ID, da_ID, nv_ID)

)
GO

-- Bảng thông báo
CREATE TABLE ThongBao (
	tb_ID INT IDENTITY(1,1),
	tb_NoiDung NVARCHAR(500) NOT NULL,
	tb_ThoiGian DATETIME DEFAULT GETDATE(),
	tb_TinhTrang BIT DEFAULT 0,
	tb_Loai NVARCHAR(20) NOT NULL, -- PhanCongCongViec, NhacNho, CapNhatTrangThai, Khac
	--
	nv_ID_NguoiNhan INT NOT NULL,
	nv_ID_NguoiGui INT NOT NULL,
	cv_ID INT NULL,
	da_ID INT NULL,
	--
	CONSTRAINT PK_ThongBao PRIMARY KEY(tb_ID),
	CONSTRAINT FK_ThongBao_NhanVien_NguoiNhan FOREIGN KEY(nv_ID_NguoiNhan) REFERENCES NhanVien(nv_ID),
	CONSTRAINT FK_ThongBao_NhanVien_NguoiGui FOREIGN KEY(nv_ID_NguoiGui) REFERENCES NhanVien(nv_ID),
	CONSTRAINT FK_ThongBao_CongViec FOREIGN KEY(cv_ID, da_ID) REFERENCES CongViec(cv_ID, da_ID)
)
GO

-----------------TRIGGER------------------
-- Trigger: Xoá NhanVien → xoá DuAn họ tạo
CREATE TRIGGER TRG_DeleteNhanVien ON NhanVien INSTEAD OF DELETE AS
BEGIN
	DELETE FROM Luong WHERE nv_ID IN (SELECT nv_ID FROM DELETED)
	DELETE FROM KPI WHERE nv_ID IN (SELECT nv_ID FROM DELETED)
	DELETE FROM CapNhatCongViec WHERE nv_ID IN (SELECT nv_ID FROM DELETED)
	DELETE FROM PhanCongCongViec WHERE nv_ID IN (SELECT nv_ID FROM DELETED)
	DELETE FROM CongViec WHERE nv_ID_NguoiTao IN (SELECT nv_ID FROM DELETED)
	DELETE FROM NhanVienThamGiaDuAn WHERE nv_ID IN (SELECT nv_ID FROM DELETED)
	DELETE FROM DuAn WHERE nv_ID_NguoiTao IN (SELECT nv_ID FROM DELETED)
	DELETE FROM NhanVien WHERE nv_ID IN (SELECT nv_ID FROM DELETED)
END
-------------

GO

-- Trigger: Xoá DuAn → xoá liên quan
CREATE TRIGGER TRG_DeleteDuAn ON DuAn INSTEAD OF DELETE AS
BEGIN
	DELETE FROM CapNhatCongViec WHERE da_ID IN (SELECT da_ID FROM DELETED)
	DELETE FROM PhanCongCongViec WHERE da_ID IN (SELECT da_ID FROM DELETED)
	DELETE FROM CongViec WHERE da_ID IN (SELECT da_ID FROM DELETED)
	DELETE FROM NhanVienThamGiaDuAn WHERE da_ID IN (SELECT da_ID FROM DELETED)
	DELETE FROM DuAn WHERE da_ID IN (SELECT da_ID FROM DELETED)
END
---------

GO

-- Trigger: Xóa nhân viên tham gia -> xóa những dữ liệu liên quan
CREATE TRIGGER TRG_DeleteNVThamGiaDA
ON NhanVienThamGiaDuAn
INSTEAD OF DELETE
AS
BEGIN
	---
	DELETE FROM CapNhatCongViec
    WHERE EXISTS (
        SELECT 1
        FROM DELETED
        WHERE CapNhatCongViec.nv_ID = DELETED.nv_ID
        AND CapNhatCongViec.da_ID = DELETED.da_ID
    );
    -- Xóa dữ liệu trong bảng PhanCongCongViec
    DELETE FROM PhanCongCongViec
    WHERE EXISTS (
        SELECT 1
        FROM DELETED
        WHERE PhanCongCongViec.nv_ID = DELETED.nv_ID
        AND PhanCongCongViec.da_ID = DELETED.da_ID
    );

    -- Xóa dữ liệu trong bảng CongViec
    DELETE FROM CongViec
    WHERE EXISTS (
        SELECT 1
        FROM DELETED
        WHERE CongViec.nv_ID_NguoiTao = DELETED.nv_ID
        AND CongViec.da_ID = DELETED.da_ID
    );

    -- Xóa dữ liệu trong bảng NhanVienThanGiaDuAn
    DELETE FROM NhanVienThamGiaDuAn
    WHERE EXISTS (
        SELECT 1
        FROM DELETED
        WHERE NhanVienThamGiaDuAn.nv_ID = DELETED.nv_ID
        AND NhanVienThamGiaDuAn.da_ID = DELETED.da_ID
    );
END;
----------------------
GO

-- Trigger: xóa công việc xóa dữ liệu liên quan
CREATE TRIGGER TRG_DeleteCongViec
ON CongViec
INSTEAD OF DELETE
AS
BEGIN
	---
	DELETE FROM CapNhatCongViec
    WHERE EXISTS (
        SELECT 1
        FROM DELETED
        WHERE CapNhatCongViec.cv_ID = DELETED.cv_ID
        AND CapNhatCongViec.da_ID = DELETED.da_ID
    );
    -- Xóa dữ liệu trong bảng PhanCongCongViec
    DELETE FROM PhanCongCongViec
    WHERE EXISTS (
        SELECT 1
        FROM DELETED
        WHERE PhanCongCongViec.cv_ID = DELETED.cv_ID
        AND PhanCongCongViec.da_ID = DELETED.da_ID
    );

    -- Xóa dữ liệu trong bảng CongViec
    DELETE FROM CongViec
    WHERE EXISTS (
        SELECT 1
        FROM DELETED
        WHERE CongViec.cv_ID = DELETED.cv_ID
        AND CongViec.da_ID = DELETED.da_ID
    );
END;
------------------
GO

--trigger xóa phân công
CREATE TRIGGER TRG_DeletePhanCongCongViec
ON PhanCongCongViec
INSTEAD OF DELETE
AS
BEGIN
	---
	DELETE FROM CapNhatCongViec
    WHERE EXISTS (
        SELECT 1
        FROM DELETED
        WHERE CapNhatCongViec.cv_ID = DELETED.cv_ID
        AND CapNhatCongViec.da_ID = DELETED.da_ID
		AND CapNhatCongViec.nv_ID = DELETED.nv_ID
    );
    -- Xóa dữ liệu trong bảng PhanCongCongViec
    DELETE FROM PhanCongCongViec
    WHERE EXISTS (
        SELECT 1
        FROM DELETED
        WHERE PhanCongCongViec.cv_ID = DELETED.cv_ID
        AND PhanCongCongViec.da_ID = DELETED.da_ID
		AND PhanCongCongViec.nv_ID = DELETED.nv_ID
    );
END;
-----------------
GO

---
--cap nhat tiep do
CREATE TRIGGER TRG_UpdateTienDoDuAn
ON CongViec
AFTER UPDATE
AS
BEGIN
    -- Chỉ thực hiện nếu trạng thái của công việc được thay đổi
    IF EXISTS (SELECT 1 FROM INSERTED WHERE tt_Ma = 'ht') -- Giả sử 'HoanThanh' là mã trạng thái hoàn thành
    BEGIN
        DECLARE @da_ID INT;
        
        -- Lấy ID dự án của công việc đã thay đổi
        SELECT @da_ID = da_ID FROM INSERTED;

        -- Cập nhật tiến độ dự án
        DECLARE @totalJobs INT, @completedJobs INT;
        
        -- Tính tổng số công việc trong dự án
        SELECT @totalJobs = COUNT(*) FROM CongViec WHERE da_ID = @da_ID;
        
        -- Tính số công việc hoàn thành
        SELECT @completedJobs = COUNT(*) FROM CongViec 
        WHERE da_ID = @da_ID AND tt_Ma = 'ht';

        -- Tính tiến độ và cập nhật vào bảng DuAn (với kiểu DECIMAL)
        IF @totalJobs > 0
        BEGIN
            UPDATE DuAn
            SET da_TienDo = CAST((@completedJobs * 100.0) / @totalJobs AS DECIMAL(5, 2))
            WHERE da_ID = @da_ID;
        END
    END
END;
-----------------------------------------


GO

--------
-- Trigger cập nhật bảng KPI khi INSERT, UPDATE hoặc DELETE bảng CongViec

CREATE TRIGGER TRG_UpdateKPI
ON CongViec
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Bảng tạm chứa tất cả nv_ID và tháng liên quan trong INSERT, UPDATE hoặc DELETE
    DECLARE @tmp TABLE (nv_ID INT, kpi_ThangNam DATE);

    -- Lấy dữ liệu từ INSERTED
    INSERT INTO @tmp (nv_ID, kpi_ThangNam)
    SELECT pc.nv_ID, DATEFROMPARTS(YEAR(i.cv_KetThuc), MONTH(i.cv_KetThuc), 1)
    FROM INSERTED i
    JOIN PhanCongCongViec pc ON pc.cv_ID = i.cv_ID AND pc.da_ID = i.da_ID;

    -- Lấy dữ liệu từ DELETED
    INSERT INTO @tmp (nv_ID, kpi_ThangNam)
    SELECT pc.nv_ID, DATEFROMPARTS(YEAR(d.cv_KetThuc), MONTH(d.cv_KetThuc), 1)
    FROM DELETED d
    JOIN PhanCongCongViec pc ON pc.cv_ID = d.cv_ID AND pc.da_ID = d.da_ID;

    -- Xóa trùng
    WITH DistinctRows AS (
        SELECT DISTINCT nv_ID, kpi_ThangNam FROM @tmp
    )
    MERGE KPI AS target
    USING DistinctRows AS source
    ON target.nv_ID = source.nv_ID AND target.kpi_ThangNam = source.kpi_ThangNam
    WHEN MATCHED THEN
        UPDATE SET target.kpi_PhanTram = (
            SELECT 
                CASE WHEN COUNT(*) = 0 THEN 0
                     ELSE CAST(SUM(CASE 
                         WHEN cv.tt_Ma = 'ht' AND cv.cv_ThoiGianHoanThanh IS NOT NULL AND cv.cv_ThoiGianHoanThanh <= cv.cv_KetThuc THEN 1 ELSE 0 END) * 100.0 
                     / COUNT(*) AS DECIMAL(5,2))
                END
            FROM CongViec cv
            JOIN PhanCongCongViec pc ON pc.cv_ID = cv.cv_ID AND pc.da_ID = cv.da_ID
            WHERE pc.nv_ID = source.nv_ID 
            AND DATEFROMPARTS(YEAR(cv.cv_KetThuc), MONTH(cv.cv_KetThuc), 1) = source.kpi_ThangNam
        )
    WHEN NOT MATCHED THEN
        INSERT (nv_ID, kpi_ThangNam, kpi_PhanTram)
        VALUES (
            source.nv_ID,
            source.kpi_ThangNam,
            (
                SELECT 
                    CASE WHEN COUNT(*) = 0 THEN 0
                         ELSE CAST(SUM(CASE 
                             WHEN cv.tt_Ma = 'ht' AND cv.cv_ThoiGianHoanThanh IS NOT NULL AND cv.cv_ThoiGianHoanThanh <= cv.cv_KetThuc THEN 1 ELSE 0 END) * 100.0 
                         / COUNT(*) AS DECIMAL(5,2))
                    END
                FROM CongViec cv
                JOIN PhanCongCongViec pc ON pc.cv_ID = cv.cv_ID AND pc.da_ID = cv.da_ID
                WHERE pc.nv_ID = source.nv_ID 
                AND DATEFROMPARTS(YEAR(cv.cv_KetThuc), MONTH(cv.cv_KetThuc), 1) = source.kpi_ThangNam
            )
        );
END

GO

-----
------------
CREATE TRIGGER TRG_UpdateKPI_PhanCong
ON PhanCongCongViec
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @tmp TABLE (nv_ID INT, kpi_ThangNam DATE);

    -- Lấy từ INSERTED
    INSERT INTO @tmp (nv_ID, kpi_ThangNam)
    SELECT i.nv_ID, DATEFROMPARTS(YEAR(cv.cv_KetThuc), MONTH(cv.cv_KetThuc), 1)
    FROM INSERTED i
    JOIN CongViec cv ON cv.cv_ID = i.cv_ID AND cv.da_ID = i.da_ID;

    -- Lấy từ DELETED
    INSERT INTO @tmp (nv_ID, kpi_ThangNam)
    SELECT d.nv_ID, DATEFROMPARTS(YEAR(cv.cv_KetThuc), MONTH(cv.cv_KetThuc), 1)
    FROM DELETED d
    JOIN CongViec cv ON cv.cv_ID = d.cv_ID AND cv.da_ID = d.da_ID;

    -- Lọc DISTINCT và chỉ lấy tháng hiện tại trở về trước
    WITH DistinctRows AS (
        SELECT DISTINCT nv_ID, kpi_ThangNam
        FROM @tmp
        WHERE kpi_ThangNam <= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)
    )
    MERGE KPI AS target
    USING DistinctRows AS source
    ON target.nv_ID = source.nv_ID AND target.kpi_ThangNam = source.kpi_ThangNam
    WHEN MATCHED THEN
        UPDATE SET target.kpi_PhanTram = (
            SELECT 
                CASE WHEN COUNT(*) = 0 THEN 0
                     ELSE CAST(SUM(CASE 
                         WHEN cv.tt_Ma = 'ht' AND cv.cv_ThoiGianHoanThanh IS NOT NULL AND cv.cv_ThoiGianHoanThanh <= cv.cv_KetThuc THEN 1 ELSE 0 END) * 100.0 
                     / COUNT(*) AS DECIMAL(5,2))
                END
            FROM CongViec cv
            JOIN PhanCongCongViec pc ON pc.cv_ID = cv.cv_ID AND pc.da_ID = cv.da_ID
            WHERE pc.nv_ID = source.nv_ID 
            AND DATEFROMPARTS(YEAR(cv.cv_KetThuc), MONTH(cv.cv_KetThuc), 1) = source.kpi_ThangNam
        )
    WHEN NOT MATCHED THEN
        INSERT (nv_ID, kpi_ThangNam, kpi_PhanTram)
        VALUES (
            source.nv_ID,
            source.kpi_ThangNam,
            0
        );
END;
----
go
DECLARE @currentMonth DATE = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);

INSERT INTO KPI (nv_ID, kpi_ThangNam, kpi_PhanTram)
SELECT nv_ID, @currentMonth, 0
FROM NhanVien
WHERE NOT EXISTS (
    SELECT 1 FROM KPI WHERE nv_ID = NhanVien.nv_ID AND kpi_ThangNam = @currentMonth
);

----
GO

--LƯƠNG
--thêm lương tính lương
-- nếu kpi <30 trừ 15% lương cơ bản, <50 trừ 10% lương cơ bản, >= 70 tính lương cơ bản + kpi/100* lương cơ bản*0.3, sau đó cộng phụ cấp (nếu có)
CREATE TRIGGER TRG_InsertUpdateLuongFromKPI
ON KPI
AFTER INSERT, UPDATE
AS
BEGIN
	-- 1. THÊM BẢNG LƯƠNG MỚI nếu chưa có cho nv_ID, ThángNăm
	INSERT INTO Luong(nv_ID, luong_ThangNam, luong_PhuCap)
	SELECT i.nv_ID, i.kpi_ThangNam, 0
	FROM inserted i
	WHERE NOT EXISTS (
		SELECT 1 FROM Luong l
		WHERE l.nv_ID = i.nv_ID AND l.luong_ThangNam = i.kpi_ThangNam
	);

	-- 2. CẬP NHẬT LƯƠNG THỰC NHẬN DỰA TRÊN KPI
	UPDATE L
	SET L.luong_ThucNhan = ROUND(
		CASE 
			WHEN K.kpi_PhanTram < 30 THEN NV.nv_LuongCoBan * 0.85
			WHEN K.kpi_PhanTram < 50 THEN NV.nv_LuongCoBan * 0.90
			WHEN K.kpi_PhanTram >= 70 THEN NV.nv_LuongCoBan + (K.kpi_PhanTram / 100.0) * NV.nv_LuongCoBan * 0.3
			ELSE NV.nv_LuongCoBan
		END + ISNULL(L.luong_PhuCap, 0), 2)
	FROM Luong L
	INNER JOIN KPI K ON L.nv_ID = K.nv_ID AND L.luong_ThangNam = K.kpi_ThangNam
	INNER JOIN NhanVien NV ON NV.nv_ID = K.nv_ID
	WHERE EXISTS (
		SELECT 1 FROM inserted i 
		WHERE i.nv_ID = K.nv_ID AND i.kpi_ThangNam = K.kpi_ThangNam
	);
END;
go

--thiết lập lương cơ bản dựa vào quyền
CREATE TRIGGER SetLuongCoBanMacDinh
ON NhanVien
AFTER INSERT
AS
BEGIN
    UPDATE NhanVien
    SET nv_LuongCoBan = q.q_LuongCoBan
    FROM NhanVien nv
    INNER JOIN Quyen q ON nv.q_Ma = q.q_Ma
    WHERE nv.nv_ID IN (SELECT nv_ID FROM inserted);
END;
GO

--Cập nhật lương cho các nhân viên có quyền đó, nhân viên đã chỉnh riêng thì q_lcb = nv_lcbm + (nv_lcbc - q_lcbc)
CREATE TRIGGER SyncLuongCoBan_Quyen
ON Quyen
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF UPDATE(q_LuongCoBan)
    BEGIN
        -- Ghi lịch sử thay đổi lương chung cho quyền
        INSERT INTO LichSuCapNhatLuong (q_Ma, nv_ID, luongCu, luongMoi, thoiGianCapNhat)
        SELECT i.q_Ma, NULL, d.q_LuongCoBan, i.q_LuongCoBan, GETDATE()
        FROM inserted i
        INNER JOIN deleted d ON i.q_Ma = d.q_Ma
        WHERE i.q_LuongCoBan != d.q_LuongCoBan;

        -- Lưu lương cũ của nhân viên vào bảng tạm để ghi lịch sử sau
        DECLARE @TempLuong TABLE (nv_ID INT, luongCu DECIMAL(15,2));

        INSERT INTO @TempLuong (nv_ID, luongCu)
        SELECT nv.nv_ID, nv.nv_LuongCoBan
        FROM NhanVien nv
        INNER JOIN inserted i ON nv.q_Ma = i.q_Ma
        WHERE i.q_LuongCoBan != (SELECT d.q_LuongCoBan FROM deleted d WHERE d.q_Ma = i.q_Ma);

        -- Cập nhật lương cho tất cả nhân viên
        UPDATE NhanVien
        SET nv_LuongCoBan = 
            CASE 
                WHEN EXISTS (
                    SELECT 1 
                    FROM LichSuCapNhatLuong ls 
                    WHERE ls.nv_ID = nv.nv_ID 
                    AND ls.q_Ma IS NULL
                ) THEN 
                    i.q_LuongCoBan + 
                    CASE 
                        WHEN nv.nv_LuongCoBan - d.q_LuongCoBan > 0 
                        THEN nv.nv_LuongCoBan - d.q_LuongCoBan 
                        ELSE 0 
                    END
                ELSE 
                    i.q_LuongCoBan
            END
        FROM NhanVien nv
        INNER JOIN inserted i ON nv.q_Ma = i.q_Ma
        INNER JOIN deleted d ON nv.q_Ma = d.q_Ma
        WHERE i.q_LuongCoBan != d.q_LuongCoBan;

        -- Ghi lịch sử cho từng nhân viên bị ảnh hưởng
        INSERT INTO LichSuCapNhatLuong (q_Ma, nv_ID, luongCu, luongMoi, thoiGianCapNhat)
        SELECT NULL, nv.nv_ID, t.luongCu, nv.nv_LuongCoBan, GETDATE()
        FROM NhanVien nv
        INNER JOIN @TempLuong t ON nv.nv_ID = t.nv_ID
        INNER JOIN inserted i ON nv.q_Ma = i.q_Ma
        WHERE i.q_LuongCoBan != (SELECT d.q_LuongCoBan FROM deleted d WHERE d.q_Ma = i.q_Ma);
    END;
END;
GO

--ghi lại lịch sử thay đổi lương cơ bản của từng nhân viên
CREATE TRIGGER LogLuongNhanVien
ON NhanVien
AFTER UPDATE
AS
BEGIN
    IF UPDATE(nv_LuongCoBan)
    BEGIN
        INSERT INTO LichSuCapNhatLuong (q_Ma, nv_ID, luongCu, luongMoi, thoiGianCapNhat)
        SELECT NULL, i.nv_ID, d.nv_LuongCoBan, i.nv_LuongCoBan, GETDATE()
        FROM inserted i
        INNER JOIN deleted d ON i.nv_ID = d.nv_ID
        WHERE i.nv_LuongCoBan != d.nv_LuongCoBan;
    END;
END;
GO

--cập nhật lương chung theo quyền
CREATE PROCEDURE CapNhatLuongTheoQuyen
    @q_Ma VARCHAR(15),
    @LuongMoi DECIMAL(15,2)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM Quyen WHERE q_Ma = @q_Ma)
            THROW 50001, N'Quyền không tồn tại.', 1;

        IF @LuongMoi < 0
            THROW 50002, N'Lương cơ bản không được âm.', 1;

        UPDATE Quyen
        SET q_LuongCoBan = @LuongMoi
        WHERE q_Ma = @q_Ma;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH;
END;
GO

--cập nhật lương cho từng nhân viên
CREATE PROCEDURE ChinhSuaLuongNhanVien
    @nv_ID INT,
    @LuongMoi DECIMAL(15,2)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM NhanVien WHERE nv_ID = @nv_ID)
            THROW 50003, N'Nhân viên không tồn tại.', 1;

        IF @LuongMoi < 0
            THROW 50004, N'Lương cơ bản không được âm.', 1;

        UPDATE NhanVien
        SET nv_LuongCoBan = @LuongMoi
        WHERE nv_ID = @nv_ID;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH;
END;
GO

--tăng hiệu xuất truy vấn
CREATE NONCLUSTERED INDEX IX_LichSuCapNhatLuong_nv_ID_q_Ma
ON LichSuCapNhatLuong (nv_ID, q_Ma)
INCLUDE (luongCu, luongMoi, thoiGianCapNhat);
go

--thông báo
-- Store procedure để thêm thông báo khi phân công công việc
CREATE OR ALTER PROCEDURE sp_ThemThongBaoPhanCongCongViec
    @cv_ID INT,
    @da_ID INT,
    @nv_ID_NguoiNhan INT,
    @nv_ID_NguoiGui INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Kiểm tra dữ liệu đầu vào
        IF NOT EXISTS (
            SELECT 1 
            FROM CongViec 
            WHERE cv_ID = @cv_ID AND da_ID = @da_ID
        )
            THROW 50001, N'Công việc không tồn tại.', 1;

        IF NOT EXISTS (
            SELECT 1 
            FROM NhanVien 
            WHERE nv_ID = @nv_ID_NguoiNhan
        )
            THROW 50002, N'Nhân viên nhận không tồn tại.', 1;

        IF NOT EXISTS (
            SELECT 1 
            FROM NhanVien 
            WHERE nv_ID = @nv_ID_NguoiGui
        )
            THROW 50003, N'Nhân viên gửi không tồn tại.', 1;

        -- Chèn thông báo
        INSERT INTO ThongBao (
            tb_NoiDung, 
            tb_ThoiGian, 
            tb_TinhTrang, 
            tb_Loai, 
            nv_ID_NguoiNhan, 
            nv_ID_NguoiGui, 
            cv_ID, 
            da_ID
        )
        SELECT 
            CONCAT(nv.nv_Ten, N' đã phân công bạn làm công việc "', cv.cv_Ten, '"'),
            GETDATE(),
            0, -- Chưa đọc
            N'PhanCongCongViec',
            @nv_ID_NguoiNhan,
            @nv_ID_NguoiGui,
            @cv_ID,
            @da_ID
        FROM CongViec cv
        INNER JOIN NhanVien nv 
            ON nv.nv_ID = @nv_ID_NguoiGui
        WHERE cv.cv_ID = @cv_ID 
            AND cv.da_ID = @da_ID;
    END TRY
    BEGIN CATCH
        -- Ghi log lỗi
        INSERT INTO ErrorLog (ErrorTime, ErrorMessage)
        VALUES (
            GETDATE(),
            CONCAT(
                N'Lỗi sp_ThemThongBaoPhanCongCongViec: ', 
                ERROR_MESSAGE(), 
                N' | cv_ID: ', @cv_ID, 
                N' | da_ID: ', @da_ID, 
                N' | nv_ID_NguoiNhan: ', @nv_ID_NguoiNhan
            )
        );

        THROW;
    END CATCH;
END;
GO

-- Thêm trigger tự động tạo thông báo khi phân công công việc
-- Tạo bảng ErrorLog nếu chưa có
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ErrorLog')
CREATE TABLE ErrorLog (
    ErrorID INT IDENTITY(1,1) PRIMARY KEY,
    ErrorTime DATETIME DEFAULT GETDATE(),
    ErrorMessage NVARCHAR(4000)
);
GO


CREATE OR ALTER TRIGGER trg_PhanCongCongViec_ThemThongBao
ON PhanCongCongViec
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Chèn thông báo cho mỗi bản ghi được thêm
        INSERT INTO ThongBao (
            tb_NoiDung, 
            tb_ThoiGian, 
            tb_TinhTrang, 
            tb_Loai, 
            nv_ID_NguoiNhan, 
            nv_ID_NguoiGui, 
            cv_ID, 
            da_ID
        )
        SELECT 
            CONCAT(nv_gui.nv_Ten, N' đã phân công bạn làm công việc "', cv.cv_Ten, '"'),
            GETDATE(),
            0, -- Chưa đọc
            N'PhanCongCongViec',
            i.nv_ID,
            cv.nv_ID_NguoiTao,
            i.cv_ID,
            i.da_ID
        FROM inserted i
        INNER JOIN CongViec cv 
            ON i.cv_ID = cv.cv_ID 
            AND i.da_ID = cv.da_ID
        INNER JOIN NhanVien nv_gui 
            ON cv.nv_ID_NguoiTao = nv_gui.nv_ID;
    END TRY
    BEGIN CATCH
        -- Ghi log lỗi chi tiết
        INSERT INTO ErrorLog (ErrorTime, ErrorMessage)
        SELECT 
            GETDATE(),
            CONCAT(
                N'Lỗi trigger trg_PhanCongCongViec_ThemThongBao: ', 
                ERROR_MESSAGE(), 
                N' | cv_ID: ', i.cv_ID, 
                N' | da_ID: ', i.da_ID, 
                N' | nv_ID: ', i.nv_ID
            )
        FROM inserted i;

        THROW;
    END CATCH;
END;
GO

-- Store procedure để cập nhật trạng thái công việc
CREATE OR ALTER PROCEDURE sp_CapNhatTrangThaiCongViec
    @cv_ID INT,
    @da_ID INT,
    @tt_Ma VARCHAR(15),
    @nv_ID_NguoiCapNhat INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Kiểm tra công việc tồn tại
        IF NOT EXISTS (
            SELECT 1 
            FROM CongViec 
            WHERE cv_ID = @cv_ID AND da_ID = @da_ID
        )
            THROW 50009, N'Công việc không tồn tại.', 1;

        -- Kiểm tra trạng thái tồn tại
        IF NOT EXISTS (
            SELECT 1 
            FROM TrangThai 
            WHERE tt_Ma = @tt_Ma
        )
            THROW 50010, N'Trạng thái không tồn tại.', 1;

        -- Kiểm tra người cập nhật là người được phân công
        IF NOT EXISTS (
            SELECT 1 
            FROM PhanCongCongViec 
            WHERE cv_ID = @cv_ID AND da_ID = @da_ID AND nv_ID = @nv_ID_NguoiCapNhat
        )
            THROW 50011, N'Nhân viên không được phân công công việc này.', 1;

        -- Cập nhật trạng thái
        UPDATE CongViec
        SET tt_Ma = @tt_Ma
        WHERE cv_ID = @cv_ID AND da_ID = @da_ID;
    END TRY
    BEGIN CATCH
        INSERT INTO ErrorLog (ErrorTime, ErrorMessage)
        VALUES (
            GETDATE(),
            CONCAT(
                N'Lỗi sp_CapNhatTrangThaiCongViec: ', 
                ERROR_MESSAGE(), 
                N' | cv_ID: ', @cv_ID, 
                N' | da_ID: ', @da_ID, 
                N' | tt_Ma: ', @tt_Ma, 
                N' | nv_ID_NguoiCapNhat: ', @nv_ID_NguoiCapNhat
            )
        );

        THROW;
    END CATCH;
END;
GO

-- Store procedure để đánh dấu thông báo đã đọc
CREATE OR ALTER PROCEDURE sp_DanhDauThongBaoDaDoc
    @tb_ID INT,
    @nv_ID_NguoiNhan INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Kiểm tra thông báo tồn tại và thuộc về người nhận
        IF NOT EXISTS (
            SELECT 1 
            FROM ThongBao 
            WHERE tb_ID = @tb_ID AND nv_ID_NguoiNhan = @nv_ID_NguoiNhan
        )
            THROW 50015, N'Thông báo không tồn tại hoặc bạn không có quyền.', 1;

        -- Cập nhật trạng thái
        UPDATE ThongBao
        SET tb_TinhTrang = 1
        WHERE tb_ID = @tb_ID AND nv_ID_NguoiNhan = @nv_ID_NguoiNhan;
    END TRY
    BEGIN CATCH
        INSERT INTO ErrorLog (ErrorTime, ErrorMessage)
        VALUES (
            GETDATE(),
            CONCAT(
                N'Lỗi sp_DanhDauThongBaoDaDoc: ', 
                ERROR_MESSAGE(), 
                N' | tb_ID: ', @tb_ID, 
                N' | nv_ID_NguoiNhan: ', @nv_ID_NguoiNhan
            )
        );

        THROW;
    END CATCH;
END;
GO

-- Store procedure để đánh dấu tất cả thông báo của người dùng đã đọc
CREATE OR ALTER PROCEDURE sp_DanhDauTatCaThongBaoDaDoc
    @nv_ID INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Kiểm tra nhân viên tồn tại
        IF NOT EXISTS (
            SELECT 1 
            FROM NhanVien 
            WHERE nv_ID = @nv_ID
        )
            THROW 50016, N'Nhân viên không tồn tại.', 1;

        -- Cập nhật chỉ thông báo chưa đọc
        UPDATE ThongBao
        SET tb_TinhTrang = 1
        WHERE nv_ID_NguoiNhan = @nv_ID AND tb_TinhTrang = 0;
    END TRY
    BEGIN CATCH
        INSERT INTO ErrorLog (ErrorTime, ErrorMessage)
        VALUES (
            GETDATE(),
            CONCAT(
                N'Lỗi sp_DanhDauTatCaThongBaoDaDoc: ', 
                ERROR_MESSAGE(), 
                N' | nv_ID: ', @nv_ID
            )
        );

        THROW;
    END CATCH;
END;
GO

-- Store procedure để lấy danh sách thông báo của người dùng
CREATE OR ALTER PROCEDURE sp_LayDanhSachThongBao
    @nv_ID INT,
    @TinhTrang BIT = NULL, -- NULL: tất cả, 0: chưa đọc, 1: đã đọc
    @SoLuong INT = 50
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Kiểm tra nhân viên tồn tại
        IF NOT EXISTS (
            SELECT 1 
            FROM NhanVien 
            WHERE nv_ID = @nv_ID
        )
            THROW 50017, N'Nhân viên không tồn tại.', 1;

        -- Lấy danh sách thông báo
        SELECT TOP (@SoLuong)
            tb.tb_ID,
            tb.tb_NoiDung,
            tb.tb_ThoiGian,
            tb.tb_TinhTrang,
            tb.tb_Loai,
            tb.nv_ID_NguoiGui,
            nv.nv_Ten AS TenNguoiGui,
            tb.cv_ID,
            tb.da_ID,
            cv.cv_Ten AS TenCongViec
        FROM ThongBao tb
        INNER JOIN NhanVien nv 
            ON tb.nv_ID_NguoiGui = nv.nv_ID
        LEFT JOIN CongViec cv 
            ON tb.cv_ID = cv.cv_ID AND tb.da_ID = cv.da_ID
        WHERE tb.nv_ID_NguoiNhan = @nv_ID
            AND (@TinhTrang IS NULL OR tb.tb_TinhTrang = @TinhTrang)
        ORDER BY tb.tb_ThoiGian DESC;
    END TRY
    BEGIN CATCH
        INSERT INTO ErrorLog (ErrorTime, ErrorMessage)
        VALUES (
            GETDATE(),
            CONCAT(
                N'Lỗi sp_LayDanhSachThongBao: ', 
                ERROR_MESSAGE(), 
                N' | nv_ID: ', @nv_ID, 
                N' | TinhTrang: ', @TinhTrang, 
                N' | SoLuong: ', @SoLuong
            )
        );

        THROW;
    END CATCH;
END;
GO

-- Store procedure để đếm số thông báo chưa đọc
CREATE OR ALTER PROCEDURE sp_DemThongBaoChuaDoc
    @nv_ID INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Kiểm tra nhân viên tồn tại
        IF NOT EXISTS (
            SELECT 1 
            FROM NhanVien 
            WHERE nv_ID = @nv_ID
        )
            THROW 50018, N'Nhân viên không tồn tại.', 1;

        -- Đếm thông báo chưa đọc
        SELECT COUNT(*) AS SoThongBaoChuaDoc
        FROM ThongBao
        WHERE nv_ID_NguoiNhan = @nv_ID AND tb_TinhTrang = 0;
    END TRY
    BEGIN CATCH
        INSERT INTO ErrorLog (ErrorTime, ErrorMessage)
        VALUES (
            GETDATE(),
            CONCAT(
                N'Lỗi sp_DemThongBaoChuaDoc: ', 
                ERROR_MESSAGE(), 
                N' | nv_ID: ', @nv_ID
            )
        );

        THROW;
    END CATCH;
END;
GO


-- Store procedure thêm thông báo nhắc nhở khi gần đến hạn
CREATE OR ALTER PROCEDURE sp_ThemThongBaoNhacNhoHanCongViec
    @SoNgayTruocHan INT = 2 -- Tham số tùy chỉnh số ngày trước hạn
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Tạo bảng tạm để lưu công việc cần nhắc nhở
        CREATE TABLE #CongViecCanNhacNho (
            cv_ID INT,
            da_ID INT,
            nv_ID_NguoiNhan INT,
            nv_ID_NguoiGui INT,
            cv_Ten NVARCHAR(50)
        );

        -- Lấy danh sách công việc gần đến hạn
        INSERT INTO #CongViecCanNhacNho (
            cv_ID, 
            da_ID, 
            nv_ID_NguoiNhan, 
            nv_ID_NguoiGui, 
            cv_Ten
        )
        SELECT 
            cv.cv_ID,
            cv.da_ID,
            pc.nv_ID,
            cv.nv_ID_NguoiTao,
            cv.cv_Ten
        FROM CongViec cv
        INNER JOIN PhanCongCongViec pc 
            ON cv.cv_ID = pc.cv_ID 
            AND cv.da_ID = pc.da_ID
        INNER JOIN TrangThai tt 
            ON cv.tt_Ma = tt.tt_Ma
        WHERE cv.cv_KetThuc IS NOT NULL
            AND DATEDIFF(DAY, GETDATE(), cv.cv_KetThuc) = @SoNgayTruocHan
            AND tt.tt_Ten != N'Hoàn thành'
            -- Tránh gửi thông báo trùng lặp trong cùng ngày
            AND NOT EXISTS (
                SELECT 1
                FROM ThongBao tb
                WHERE tb.cv_ID = cv.cv_ID
                    AND tb.da_ID = cv.da_ID
                    AND tb.nv_ID_NguoiNhan = pc.nv_ID
                    AND tb.tb_Loai = N'NhacNho'
                    AND CAST(tb.tb_ThoiGian AS DATE) = CAST(GETDATE() AS DATE)
            );

        -- Chèn thông báo
        INSERT INTO ThongBao (
            tb_NoiDung, 
            tb_ThoiGian, 
            tb_TinhTrang, 
            tb_Loai, 
            nv_ID_NguoiNhan, 
            nv_ID_NguoiGui, 
            cv_ID, 
            da_ID
        )
        SELECT 
            CONCAT(
                N'Công việc "', 
                temp.cv_Ten, 
                N'" sẽ hết hạn trong ', 
                @SoNgayTruocHan, 
                N' ngày nữa (', 
                CONVERT(NVARCHAR, cv.cv_KetThuc, 103), 
                N')'
            ),
            GETDATE(),
            0, -- Chưa đọc
            N'NhacNho',
            temp.nv_ID_NguoiNhan,
            temp.nv_ID_NguoiGui,
            temp.cv_ID,
            temp.da_ID
        FROM #CongViecCanNhacNho temp
        INNER JOIN CongViec cv 
            ON cv.cv_ID = temp.cv_ID 
            AND cv.da_ID = temp.da_ID;

        -- Xóa bảng tạm
        DROP TABLE #CongViecCanNhacNho;
    END TRY
    BEGIN CATCH
        -- Ghi log lỗi
        INSERT INTO ErrorLog (ErrorTime, ErrorMessage)
        VALUES (
            GETDATE(),
            CONCAT(
                N'Lỗi sp_ThemThongBaoNhacNhoHanCongViec: ', 
                ERROR_MESSAGE(), 
                N' | SoNgayTruocHan: ', @SoNgayTruocHan
            )
        );

        -- Xóa bảng tạm nếu còn tồn tại
        IF OBJECT_ID('tempdb..#CongViecCanNhacNho') IS NOT NULL
            DROP TABLE #CongViecCanNhacNho;

        THROW;
    END CATCH;
END;
GO

-- Store procedure tạo thông báo khi cập nhật công việc
CREATE OR ALTER PROCEDURE sp_ThemThongBaoCapNhatCongViec
    @cn_ID INT,
    @cv_ID INT,
    @da_ID INT,
    @nv_ID_NguoiCapNhat INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Kiểm tra dữ liệu đầu vào
        IF NOT EXISTS (
            SELECT 1 
            FROM CapNhatCongViec 
            WHERE cn_ID = @cn_ID 
                AND cv_ID = @cv_ID 
                AND da_ID = @da_ID 
                AND nv_ID = @nv_ID_NguoiCapNhat
        )
            THROW 50005, N'Cập nhật công việc không tồn tại.', 1;

        IF NOT EXISTS (
            SELECT 1 
            FROM CongViec 
            WHERE cv_ID = @cv_ID 
                AND da_ID = @da_ID
        )
            THROW 50006, N'Công việc không tồn tại.', 1;

        IF NOT EXISTS (
            SELECT 1 
            FROM NhanVien 
            WHERE nv_ID = @nv_ID_NguoiCapNhat
        )
            THROW 50007, N'Nhân viên cập nhật không tồn tại.', 1;

        -- Chèn thông báo cho người tạo và người được phân công
        INSERT INTO ThongBao (
            tb_NoiDung, 
            tb_ThoiGian, 
            tb_TinhTrang, 
            tb_Loai, 
            nv_ID_NguoiNhan, 
            nv_ID_NguoiGui, 
            cv_ID, 
            da_ID
        )
        SELECT 
            CONCAT(
                nv_capnhat.nv_Ten, 
                N' đã cập nhật tiến độ công việc "', 
                cv.cv_Ten, 
                N'": ', 
                cn.cn_MoTa
            ),
            GETDATE(),
            0, -- Chưa đọc
            N'CapNhatCongViec',
            recipients.nv_ID,
            @nv_ID_NguoiCapNhat,
            @cv_ID,
            @da_ID
        FROM CongViec cv
        INNER JOIN CapNhatCongViec cn 
            ON cn.cv_ID = @cv_ID 
            AND cn.da_ID = @da_ID 
            AND cn.cn_ID = @cn_ID
        INNER JOIN NhanVien nv_capnhat 
            ON nv_capnhat.nv_ID = @nv_ID_NguoiCapNhat
        CROSS APPLY (
            -- Người tạo công việc
            SELECT cv.nv_ID_NguoiTao AS nv_ID
            WHERE cv.nv_ID_NguoiTao != @nv_ID_NguoiCapNhat
            UNION
            -- Người được phân công
            SELECT pc.nv_ID
            FROM PhanCongCongViec pc
            WHERE pc.cv_ID = @cv_ID 
                AND pc.da_ID = @da_ID 
                AND pc.nv_ID != @nv_ID_NguoiCapNhat
        ) recipients
        WHERE cv.cv_ID = @cv_ID 
            AND cv.da_ID = @da_ID;
    END TRY
    BEGIN CATCH
        -- Ghi log lỗi
        INSERT INTO ErrorLog (ErrorTime, ErrorMessage)
        VALUES (
            GETDATE(),
            CONCAT(
                N'Lỗi sp_ThemThongBaoCapNhatCongViec: ', 
                ERROR_MESSAGE(), 
                N' | cn_ID: ', @cn_ID, 
                N' | cv_ID: ', @cv_ID, 
                N' | da_ID: ', @da_ID, 
                N' | nv_ID_NguoiCapNhat: ', @nv_ID_NguoiCapNhat
            )
        );

        THROW;
    END CATCH;
END;
GO

-- Trigger để tự động tạo thông báo khi có cập nhật công việc mới
CREATE OR ALTER TRIGGER trg_CapNhatCongViec_ThemThongBao
ON CapNhatCongViec
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @cn_ID INT, @cv_ID INT, @da_ID INT, @nv_ID INT;

        -- Lấy thông tin từ inserted
        SELECT @cn_ID = cn_ID, 
               @cv_ID = cv_ID, 
               @da_ID = da_ID, 
               @nv_ID = nv_ID
        FROM inserted;

        -- Kiểm tra dữ liệu trước khi gọi stored procedure
        IF NOT EXISTS (
            SELECT 1 
            FROM CongViec 
            WHERE cv_ID = @cv_ID AND da_ID = @da_ID
        )
            THROW 50004, N'Công việc không tồn tại trong trigger cập nhật.', 1;

        -- Gọi stored procedure
        EXEC sp_ThemThongBaoCapNhatCongViec 
            @cn_ID, 
            @cv_ID, 
            @da_ID, 
            @nv_ID;
    END TRY
    BEGIN CATCH
        -- Ghi log lỗi
        INSERT INTO ErrorLog (ErrorTime, ErrorMessage)
        VALUES (
            GETDATE(),
            CONCAT(
                N'Lỗi trigger trg_CapNhatCongViec_ThemThongBao: ', 
                ERROR_MESSAGE(), 
                N' | cn_ID: ', @cn_ID, 
                N' | cv_ID: ', @cv_ID, 
                N' | da_ID: ', @da_ID, 
                N' | nv_ID: ', @nv_ID
            )
        );

        THROW;
    END CATCH;
END;
GO

------------------------------------------------------------------------------------------
----NHẬP DỮ LIỆU----------------
delete NhanVien

DBCC CHECKIDENT ('NhanVien', RESEED, 0);
GO
DBCC CHECKIDENT ('DuAn', RESEED, 0);
GO
DBCC CHECKIDENT ('CongViec', RESEED, 0);
GO
DBCC CHECKIDENT ('CapNhatCongViec', RESEED, 0);
GO
DBCC CHECKIDENT ('NhanVien', RESEED, 0);
GO
DBCC CHECKIDENT ('DuAn', RESEED, 0);
GO
DBCC CHECKIDENT ('CongViec', RESEED, 0);
GO
DBCC CHECKIDENT ('CapNhatCongViec', RESEED, 0);
-- nếu có thêm bảng nào dùng IDENTITY thì add thêm vào

INSERT INTO Quyen (q_Ma, q_Ten, q_LuongCoBan) VALUES 
('admin', N'Quản trị hệ thống', 15000000),
('nv', N'Nhân viên', 10000000);

GO

INSERT INTO VaiTro (vt_Ma, vt_Ten) VALUES 
('ql', N'Quản lý'),
('tv', N'Thành viên');

GO

INSERT INTO TrangThai (tt_Ma, tt_Ten) VALUES 
('cht', N'Chưa thực hiện'),
('dth', N'Đang thực hiện'),
('ht', N'Hoàn thành');

GO

INSERT INTO NhanVien(nv_Ten, nv_GioiTinh, nv_NgaySinh, nv_SDT, nv_DiaChi, nv_Email, nv_TaiKhoan, nv_MatKhau, nv_LuongCoBan, q_Ma)
VALUES
(N'Admin', N'Nam', '20000101', '0911111111', N'Cần Thơ', 'admin@ql.com', 'admin', 'admin123', 30000000, 'admin'),
(N'Nguyễn A', N'Nam', '19990512', '0901234567', N'Hà Nội', 'a@ql.com', 'auser', 'pass1', 15000000, 'nv'),
(N'Trần B', N'Nữ', '19980322', '0902345678', N'Sài Gòn', 'b@ql.com', 'buser', 'pass2', 16000000, 'nv'),
(N'Lê C', N'Nam', '19970115', '0903456789', N'Đà Nẵng', 'c@ql.com', 'cuser', 'pass3', 14000000, 'nv'),
(N'Phạm D', N'Nữ', '19991230', '0904567890', N'Vũng Tàu', 'd@ql.com', 'duser', 'pass4', 15500000, 'nv'),
(N'Võ E', N'Nam', '19981010', '0905678901', N'Cần Thơ', 'e@ql.com', 'euser', 'pass5', 15800000, 'nv'),
(N'Đỗ F', N'Nữ', '19990505', '0906789012', N'Hà Nội', 'f@ql.com', 'fuser', 'pass6', 16200000, 'nv'),
(N'Ngô G', N'Nam', '19970202', '0907890123', N'Quảng Ninh', 'g@ql.com', 'guser', 'pass7', 14500000, 'nv'),
(N'Hồ H', N'Nữ', '19991111', '0908901234', N'Thái Bình', 'h@ql.com', 'huser', 'pass8', 16800000, 'nv'),
(N'Dương I', N'Nam', '19980404', '0909012345', N'Hải Phòng', 'i@ql.com', 'iuser', 'pass9', 15300000, 'nv');
select * from NhanVien
GO

INSERT INTO ThongTinCongTy (cty_Ten, cty_DiaChi, cty_SDT, cty_MoTa) VALUES
(N'Công ty TNHH CN Phần mềm Phúc Lam Phương', N'M66, Đinh Tiên Hoàng, Phường 8, TP Vĩnh Long', '0909141661 - 0796822269', N'Chuyên: Phần mềm quản lý, thiết kế website, camera quan sát,laptop, PC, pin mặt trời,...');
go

INSERT INTO DuAn (da_Ten, da_MoTa, da_BatDau, da_KetThuc, da_File, da_Path, da_TienDo, da_ThoiGianHoanThanh, nv_ID_NguoiTao, tt_Ma) VALUES
(N'Dự án A', N'Phát triển website', '2025-01-01', '2025-12-31', '', '', 0, NULL, 1, 'cht'),
(N'Dự án B', N'Ứng dụng di động', '2025-02-01', '2025-11-30', '', '', 0, NULL, 2, 'cht'),
(N'Dự án C', N'Hệ thống ERP', '2025-03-01', '2026-01-31', '', '', 80, NULL, 3, 'ht'),
(N'Dự án D', N'Phân tích dữ liệu', '2025-04-01', '2025-10-31', '', '', 20, NULL, 4, 'dth'),
(N'Dự án E', N'Tích hợp AI', '2025-05-01', '2025-09-30', '', '', 0, NULL, 5, 'cht'),
(N'Dự án F', N'Hệ thống CRM', '2025-06-01', '2025-08-31', '', '', 80, NULL, 6, 'ht'),
(N'Dự án G', N'Ứng dụng IoT', '2025-07-01', '2025-07-31', '', '', 30, NULL, 7, 'dth'),
(N'Dự án H', N'Website thương mại', '2025-08-01', '2025-06-30', '', '', 60, NULL, 8, 'dth'),
(N'Dự án I', N'Hệ thống bảo mật', '2025-09-01', '2025-05-31', '', '', 0, NULL, 9, 'cht'),
(N'Dự án J', N'Ứng dụng học tập', '2025-10-01', '2025-04-30', '', '', 0, '2025-04-01', 10, 'cht');
select * from DuAn
GO

-- 👤 Admin làm quản lý toàn bộ dự án
INSERT INTO NhanVienThamGiaDuAn (nv_ID, da_ID, vt_Ma)
VALUES
(1, 1, 'ql'), (1, 2, 'ql'), (1, 3, 'ql'), (1, 4, 'ql'), (1, 5, 'ql'),
(1, 6, 'ql'), (1, 7, 'ql'), (1, 8, 'ql'), (1, 9, 'ql'), (1, 10, 'ql');

-- 📌 Các thành viên khác tham gia (vai trò ql hoặc tv, mỗi dự án thêm 1–3 người)
INSERT INTO NhanVienThamGiaDuAn (nv_ID, da_ID, vt_Ma)
VALUES
-- Dự án 1
(2, 1, 'tv'), (3, 1, 'ql'),
-- Dự án 2
(2, 2, 'ql'), (4, 2, 'tv'),
-- Dự án 3
(5, 3, 'ql'), (6, 3, 'tv'),
-- Dự án 4
(7, 4, 'ql'), (2, 4, 'tv'),
-- Dự án 5
(8, 5, 'ql'), (3, 5, 'tv'),
-- Dự án 6
(4, 6, 'ql'), (5, 6, 'tv'),
-- Dự án 7
(6, 7, 'ql'), (7, 7, 'tv'),
-- Dự án 8
(2, 8, 'ql'), (8, 8, 'tv'),
-- Dự án 9
(9, 9, 'ql'), (10, 9, 'tv'),
-- Dự án 10
(4, 10, 'tv'), (5, 10, 'ql');

-- Bổ sung thêm nhân viên tham gia mỗi dự án, vai trò là thành viên 'tv'
INSERT INTO NhanVienThamGiaDuAn (nv_ID, da_ID, vt_Ma)
VALUES
-- Dự án 1
(4, 1, 'tv'),
(5, 1, 'tv'),
(6, 1, 'tv'),

-- Dự án 2
(5, 2, 'tv'),
(6, 2, 'tv'),
(7, 2, 'tv'),

-- Dự án 3
(2, 3, 'tv'),
(3, 3, 'tv'),
(7, 3, 'tv'),

-- Dự án 4
(6, 4, 'tv'),
(8, 4, 'tv'),
(9, 4, 'tv'),

-- Dự án 5
(6, 5, 'tv'),
(9, 5, 'tv'),
(10, 5, 'tv'),

-- Dự án 6
(6, 6, 'tv'),
(7, 6, 'tv'),
(9, 6, 'tv'),

-- Dự án 7
(8, 7, 'tv'),
(9, 7, 'tv'),
(10, 7, 'tv'),

-- Dự án 8
(3, 8, 'tv'),
(4, 8, 'tv'),
(5, 8, 'tv'),

-- Dự án 9
(4, 9, 'tv'),
(5, 9, 'tv'),
(6, 9, 'tv'),

-- Dự án 10
(6, 10, 'tv'),
(7, 10, 'tv'),
(8, 10, 'tv');

go

INSERT INTO CongViec (cv_Ten, nv_ID_NguoiTao, da_ID, tt_Ma, cv_KetThuc)
VALUES
-- Dự án 1
(N'Phân tích yêu cầu', 1, 1, 'dth', '20250429'),
(N'Lên kế hoạch triển khai', 3, 1, 'ht', '20250429'),
-- Dự án 2
(N'Thiết kế UI/UX', 1, 2, 'dth', '20250429'),
(N'Xây dựng cơ sở dữ liệu', 2, 2, 'cht', '20250429'),
-- Dự án 3
(N'Khảo sát người dùng', 1, 3, 'cht', '20250429'),
(N'Thiết kế kiến trúc phần mềm', 5, 3, 'cht', '20250429'),
-- Dự án 4
(N'Triển khai Backend', 1, 4, 'dth', '20250429'),
(N'Kiểm thử chức năng', 7, 4, 'dth', '20250429'),
-- Dự án 5
(N'Xây dựng API', 1, 5, 'cht', '20250429'),
(N'Thiết lập CI/CD', 8, 5, 'cht', '20250429'),
-- Dự án 6
(N'Đánh giá hiệu năng', 4, 6, 'cht', '20250429'),
-- Dự án 7
(N'Viết tài liệu hướng dẫn', 6, 7, 'cht', '20250429'),
-- Dự án 8
(N'Kiểm thử hệ thống', 1, 8, 'dth', '20250429'),
-- Dự án 9
(N'Fix bugs', 1, 9, 'cht', '20250429'),
-- Dự án 10
(N'Đào tạo người dùng', 1, 10, 'dth', '20250429');

GO


INSERT INTO PhanCongCongViec (cv_ID, da_ID, nv_ID)
VALUES
-- Dự án 1
(1, 1, 3),  -- Phân tích yêu cầu -> Trần B (ql)
(2, 1, 2),  -- Lên kế hoạch triển khai -> Nguyễn A (tv)

-- Dự án 2
(3, 2, 4),  -- Thiết kế UI/UX -> Lê C (tv)
(4, 2, 2),  -- Xây dựng CSDL -> Nguyễn A (ql)

-- Dự án 3
(5, 3, 5),  -- Khảo sát người dùng -> Phạm D (ql)
(6, 3, 6),  -- Thiết kế kiến trúc phần mềm -> Võ E (tv)

-- Dự án 4
(7, 4, 7),  -- Triển khai Backend -> Đỗ F (ql)
(8, 4, 2),  -- Kiểm thử chức năng -> Nguyễn A (tv)

-- Dự án 5
(9, 5, 8),  -- Xây dựng API -> Ngô G (ql)
(10, 5, 3), -- Thiết lập CI/CD -> Trần B (tv)

-- Dự án 6
(11, 6, 4), -- Đánh giá hiệu năng -> Lê C (ql)

-- Dự án 7
(12, 7, 6), -- Viết tài liệu hướng dẫn -> Võ E (ql)

-- Dự án 8
(13, 8, 2), -- Kiểm thử hệ thống -> Nguyễn A (ql)

-- Dự án 9
(14, 9, 9), -- Fix bugs -> Hồ H (ql)

-- Dự án 10
(15, 10, 5); -- Đào tạo người dùng -> Phạm D (ql)

--------------------------------------
---------------------------------------
DECLARE @currentMonth DATE = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);

INSERT INTO KPI (nv_ID, kpi_ThangNam, kpi_PhanTram)
SELECT nv_ID, @currentMonth, 0
FROM NhanVien
WHERE NOT EXISTS (
    SELECT 1 FROM KPI WHERE nv_ID = NhanVien.nv_ID AND kpi_ThangNam = @currentMonth
);

-----------------
----------------------


select * from KPI
select nv_LuongCoBan, L.* from Luong L
JOIN NHANVIEN NV ON NV.nv_ID = L.nv_ID
---------
select * from CongViec


select CV.cv_ID, CV.cv_Ten, cv_BatDau, cv_KetThuc, NT.nv_Ten as 'Người tạo việc', Da.da_Ten, Nv.nv_Ten as 'Nguoi nhan viec', tt_Ten from PhanCongCongViec PCCV
JOIN NhanVien NV ON NV.nv_ID = PCCV.nv_ID
JOIN DuAn DA ON DA.da_ID = PCCV.da_ID
JOIN CongViec CV ON CV.cv_ID = PCCV.cv_ID
JOIN NhanVien NT ON NT.nv_ID = CV.nv_ID_NguoiTao
JOIN TrangThai TT ON TT.tt_Ma = CV.tt_Ma
where MONTH(cv_KetThuc) = 4


select cv_Ten, da_Ten, nv_Ten from CongViec CV
JOIN DuAn DA ON DA.da_ID = CV.da_ID
JOIN NhanVien NV ON NV.nv_ID = CV.nv_ID_NguoiTao

select nv_Ten, KPI.* from KPI 
JOIN Nhanvien NV ON NV.nv_ID = KPI.nv_ID 
where MONTH(kpi_ThangNam) = 4 AND YEAR(kpi_ThangNam)= 2025
order by kpi_ThangNam

select * from Luong where MONTH(luong_ThangNam) = 4 AND YEAR(luong_ThangNam)= 2025

update CongViec
set tt_Ma = 'ht'
where cv_ID = 6
delete PhanCOngCOngViec
select * from KPI
select * from Luong




DECLARE @currentMonth DATE = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);

INSERT INTO KPI (nv_ID, kpi_ThangNam, kpi_PhanTram)
SELECT nv_ID, @currentMonth, 0
FROM NhanVien
WHERE NOT EXISTS (
    SELECT 1 FROM KPI WHERE nv_ID = NhanVien.nv_ID AND kpi_ThangNam = @currentMonth
);
