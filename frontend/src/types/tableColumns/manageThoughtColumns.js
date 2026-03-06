import { createColumnHelper } from '@tanstack/vue-table'
import { format } from 'date-fns'

const columnHelper = createColumnHelper()

export const getManageThoughtColumns = () => [
  columnHelper.accessor('id', {
    header: () => 'Thought_Id',
    cell: (info) => info.getValue(),
  }),

  columnHelper.group({
    header: 'Author',
    columns: [
      columnHelper.accessor('user.id', {
        header: () => 'User_Id',
        cell: (info) => info.getValue(),
      }),

      columnHelper.accessor('user.username', {
        header: () => 'Username',
        cell: (info) => info.getValue(),
      }),
    ],
  }),

  columnHelper.group({
    header: 'Content',
    columns: [
      columnHelper.accessor('title', {
        cell: (info) => info.getValue(),
      }),

      columnHelper.accessor('content', {
        cell: (info) => info.getValue(),
      }),

      columnHelper.accessor('isPublic', {
        header: () => 'Visibility',
        cell: (info) => (info.getValue() ? 'Public' : 'Private'),
      }),
    ],
  }),

  columnHelper.group({
    header: 'Interactions',
    columns: [
      columnHelper.accessor('comments.count', {
        cell: (info) => info.getValue(),
        header: () => 'Comments',
      }),

      columnHelper.accessor('reactions', {
        cell: (info) => info.getValue().reduce((acc, curr) => (acc += curr.count), 0),
        header: () => 'Reactions',
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

      columnHelper.accessor('updatedAtUtc', {
        header: () => 'Updated',
        cell: (info) =>
          new Date(info.getValue()).getUTCFullYear() > 1
            ? format(info.getValue() + 'Z', 'yyyy.MM.dd (HH:mm:ss)')
            : 'Never',
      }),
    ],
  }),
]
