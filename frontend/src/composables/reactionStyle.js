import reactionTypes from '@/types/reaction.types.js'
import { ref, toValue, watchEffect } from 'vue'

export function useReactionStyle(reactionId) {
  const iconStyle = ref('')
  const setStyle = () => {
    iconStyle.value = reactionTypes[toValue(reactionId)]
  }

  watchEffect(() => {
    setStyle()
  })

  return { iconStyle }
}
