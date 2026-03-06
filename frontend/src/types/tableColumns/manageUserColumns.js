import { createColumnHelper } from '@tanstack/vue-table'
import { format } from 'date-fns'

const columnHelper = createColumnHelper()

export const getManageUserColumns = () => [
  columnHelper.group({
    header: 'Details',
    columns: [
      columnHelper.accessor('id', {
        header: () => 'User_Id',
        // todo | make this a router link?
        cell: (info) => info.getValue(),
      }),

      columnHelper.accessor('username', {
        header: () => 'Username',
        cell: (info) => info.getValue(),
      }),
    ],
  }),

  columnHelper.group({
    header: 'Stats',
    columns: [
      columnHelper.accessor('stats.thoughts.count', {
        header: () => 'Thoughts',
        cell: (info) => info.getValue(),
      }),

      columnHelper.accessor('stats.comments.count', {
        header: () => 'Comments',
        cell: (info) => info.getValue(),
      }),

      columnHelper.accessor('stats.reactions', {
        header: () => 'Reactions',
        cell: (info) => info.getValue().reduce((acc, curr) => (acc += curr.count), 0),
      }),
    ],
  }),

  columnHelper.group({
    header: 'Timestamps',
    columns: [
      columnHelper.accessor('createdAtUtc', {
        header: () => 'Created',
        cell: (info) => format(info.getValue() + 'Z', 'yyyy.MM.dd (HH:mm:ss)'),
      }),
    ],
  }),
]
