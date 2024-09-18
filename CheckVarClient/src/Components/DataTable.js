import * as React from "react";
import { DataGrid } from "@mui/x-data-grid";
import Paper from "@mui/material/Paper";

const columns = [
  { field: "DocNo", headerName: "STT/Mã giao dịch", flex: 2 },
  { field: "Credit", headerName: "Số tiền", flex: 3 },
  { field: "TransactionsInDetail", headerName: "Chi tiết giao dịch", flex: 5 },
];

const paginationModel = { page: 0, pageSize: 5 };

export default function DataTable({ rows }) {
  return (
    <Paper sx={{ height: "100%", width: "100%" }}>
      <DataGrid
        rows={rows}
        columns={columns}
        initialState={{ pagination: { paginationModel } }}
        pageSizeOptions={[5, 10, 15, 20]}
        checkboxSelection
        sx={{ border: 0 }}
        getRowId={(row) => row["DocNo"]} // Use 'Doc No' as the row id
      />
    </Paper>
  );
}


