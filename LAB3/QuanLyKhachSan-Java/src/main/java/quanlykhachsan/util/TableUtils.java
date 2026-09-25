package quanlykhachsan.util;

import javax.swing.table.DefaultTableModel;
import javax.swing.JTable;
import java.util.List;
import java.util.Map;
import java.util.LinkedHashSet;

public class TableUtils {
    public static void fill(JTable table, List<Map<String, Object>> rows) {
        DefaultTableModel model = new DefaultTableModel() {
            @Override public boolean isCellEditable(int r, int c) { return false; }
        };
        if (rows.isEmpty()) {
            table.setModel(model);
            return;
        }
        LinkedHashSet<String> cols = new LinkedHashSet<>(rows.get(0).keySet());
        for (String c : cols) model.addColumn(c);
        for (Map<String, Object> row : rows) {
            Object[] values = new Object[cols.size()];
            int i = 0;
            for (String c : cols) values[i++] = row.get(c);
            model.addRow(values);
        }
        table.setModel(model);
    }
}