// src/composables/useApi.js
// Generic composable for async API calls with loading + error state

import { ref } from 'vue'
import { useToast } from './useToast'

export function useApi() {
  const loading = ref(false)
  const error   = ref(null)
  const toast   = useToast()

  /**
   * Execute an API call with automatic loading/error handling
   * @param {Function} fn          - async function returning axios response
   * @param {Object}   opts
   * @param {string}   opts.successMsg  - toast on success
   * @param {string}   opts.errorMsg    - toast on error (overrides API message)
   * @param {boolean}  opts.silent      - don't show error toast
   * @returns {any}    response data or null on failure
   */
  async function execute(fn, opts = {}) {
    loading.value = true
    error.value   = null

    try {
      const response = await fn()
      const data = response?.data

      if (opts.successMsg) toast.success(opts.successMsg)
      return data
    } catch (e) {
      const msg = opts.errorMsg
        || e?.response?.data?.message
        || e?.message
        || 'Произошла ошибка'

      error.value = msg

      if (!opts.silent) toast.error(msg)

      return null
    } finally {
      loading.value = false
    }
  }

  return { loading, error, execute }
}
