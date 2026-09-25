package quanlykhachsan.data;

import java.sql.*;
import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

public class Db {
    private static final String URL =
        "jdbc:sqlserver://localhost:1433;databaseName=QuanLyKhachSan;" +
        "user=sa;password=123;encrypt=true;trustServerCertificate=true";

    public static Connection open() throws SQLException {
        return DriverManager.getConnection(URL);
    }

    // ===== Không transaction (tự mở/đóng connection riêng) =====

    public static List<Map<String, Object>> query(String sql, Object... params) {
        try (Connection cn = open()) {
            return query(cn, sql, params);
        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }

    public static int execute(String sql, Object... params) {
        try (Connection cn = open()) {
            return execute(cn, sql, params);
        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }

    public static Object scalar(String sql, Object... params) {
        try (Connection cn = open()) {
            return scalar(cn, sql, params);
        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }

    // ===== Có transaction (dùng chung 1 Connection do Service mở, không tự đóng) =====

    public static List<Map<String, Object>> query(Connection cn, String sql, Object... params) {
        List<Map<String, Object>> rows = new ArrayList<>();
        try (PreparedStatement ps = cn.prepareStatement(sql)) {
            bind(ps, params);
            try (ResultSet rs = ps.executeQuery()) {
                ResultSetMetaData md = rs.getMetaData();
                int cols = md.getColumnCount();
                while (rs.next()) {
                    Map<String, Object> row = new LinkedHashMap<>();
                    for (int i = 1; i <= cols; i++) {
                        row.put(md.getColumnLabel(i), rs.getObject(i));
                    }
                    rows.add(row);
                }
            }
        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
        return rows;
    }

    public static int execute(Connection cn, String sql, Object... params) {
        try (PreparedStatement ps = cn.prepareStatement(sql)) {
            bind(ps, params);
            return ps.executeUpdate();
        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }

    public static Object scalar(Connection cn, String sql, Object... params) {
        try (PreparedStatement ps = cn.prepareStatement(sql)) {
            bind(ps, params);
            try (ResultSet rs = ps.executeQuery()) {
                return rs.next() ? rs.getObject(1) : null;
            }
        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }

    private static void bind(PreparedStatement ps, Object... params) throws SQLException {
        if (params == null) return;
        for (int i = 0; i < params.length; i++) {
            ps.setObject(i + 1, params[i]);
        }
    }
}