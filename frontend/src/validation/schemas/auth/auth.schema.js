import Joi from 'joi'
import { email, username, password } from './auth.variables.schema.js'

const login = Joi.object({
  email,
  password,
})

const signup = Joi.object({
  email,
  username,
  password,
  confirmPassword: Joi.any()
    .equal(Joi.ref('password'))
    .required()
    .label('Confirm password')
    .messages({
      'any.required': 'Confirm password is required.',
      'any.only': 'Passwords do not match.',
    }),
})

export default { login, signup }
