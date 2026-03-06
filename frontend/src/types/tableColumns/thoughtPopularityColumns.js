import { createColumnHelper } from '@tanstack/vue-table'
import { formatDistanceToNow } from 'date-fns'

const columnHelper = createColumnHelper()

export const getThoughtPopularityColumns = () => {
  return [
    columnHelper.group({
      header: 'Author',
      columns: [
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
      ],
    }),

    columnHelper.group({
      header: 'Interactions',
      columns: [
        columnHelper.accessor('commentCount', {
          cell: (info) => info.getValue(),
          header: () => 'Comments',
        }),

        columnHelper.accessor('reactions', {
          cell: (info) => info.getValue().reduce((acc, curr) => (acc += curr.count), 0),
          header: () => 'Reactions',
        }),
      ],
    }),

    columnHelper.accessor('createdAtUtc', {
      header: () => 'Posted at',
      cell: (info) =>
        formatDistanceToNow(info.getValue() + 'Z', {
          addSuffix: true,
        }),
    }),
  ]
}
